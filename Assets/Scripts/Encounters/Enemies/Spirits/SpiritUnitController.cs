using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Animation;
using Common.Grid;
using Events;
using HUD.Encounter.HoverDetails;
using Optional;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units;
using UnityEngine;

namespace Encounters.Enemies.Spirits {
  public class SpiritUnitController : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider, IDirectionalAnimatable {
    [SerializeField] protected SpiritCollection spiritsInEncounter;
    [SerializeField] private int damageOnCollision = 2;
    [SerializeField] private float speedUnitsPerSec = 2;
    
    private UnitEncounterState _encounterState;
    private Option<Bones> _targetBones;
    private SpriteRenderer _renderer;
    private List<FacingDirection> _plannedMovement;

    public FacingDirection FacingDirection { get; set; }
    public string AnimationState { get; private set; }

    public event IDirectionalAnimatable.RequestOneOffAnimation OneOffAnimation;

    public Option<Bones> TargetBones {
      get => _targetBones;
      set {
        _targetBones = value;
        if (_targetBones.HasValue) {
          PrePlanAction();
        }
      }
    }
    public Vector3Int Position {
      get => _encounterState.position;
      set => _encounterState.position = value;
    }
    
    public bool BlocksAllMovement => false;
    public bool ClaimsTile => true;
    public bool BlocksLineOfSight => false;

    private void Awake() {
      _plannedMovement = new();
      _renderer = GetComponent<SpriteRenderer>();
      AnimationState = "idle";
    }

    private void OnEnable() {
      spiritsInEncounter.Add(this);
      Dispatch.Encounters.EnemyTurnPreEnd.RegisterListener(OnEnemyTurnPreEnd);
      Dispatch.Encounters.BonesCollected.RegisterListener(OnBonesCollected);
    }

    private void OnDisable() {
      spiritsInEncounter.Remove(this);
      Dispatch.Encounters.EnemyTurnPreEnd.UnregisterListener(OnEnemyTurnPreEnd);
      Dispatch.Encounters.BonesCollected.UnregisterListener(OnBonesCollected);
    }
    
    public void Init(UnitEncounterState encounterState) {
      _encounterState = encounterState;
      transform.position = GridUtils.CellCenterWorld(encounterState.position);
    }

    public IEnumerator ExecuteMovementPlan() {
      yield return ExecuteMovement(_plannedMovement);
      _plannedMovement.Clear();
    }

    public IEnumerator ExecuteMovement(List<FacingDirection> path) {
      if (path.Count == 0) {
        yield break;
      }

      var currentPosition = Position;
      var currentIndex = 0;
      while (currentIndex < path.Count) {
        FacingDirection = path[currentIndex];
        var targetPosition = currentPosition + (Vector3Int)path[currentIndex].ToUnitVector();
        yield return MoveBetween(currentPosition, targetPosition);
        currentPosition = targetPosition;
        yield return OnPassOverTile(currentPosition);
        
        var targetTileIsClaimed = false;
        if (SceneTerrain.TryGetComponentAtTile<IPlacedOnGrid>(currentPosition, out var actor)) {
          if (actor.ClaimsTile) {
            targetTileIsClaimed = true;
            if (currentIndex == path.Count - 1) {
              // Never stop inside an occupied space. If we would, just keep going until we're not.
              path.Add(path[currentIndex]);
            }

            if (actor is EncounterActor encounterActor) {
              OnPassThroughUnit(encounterActor);
            }
          }
        }
        
        if (!targetTileIsClaimed && SceneTerrain.TryGetComponentAtTile<Bones>(currentPosition, out var bones)) {
          if (TargetBones.Contains(bones)) {
            yield return DissipateAndHandleBones(bones);
            yield break;
          }
        }
        
        currentIndex++;
      }
      
      Position = currentPosition;
    }

    public List<Vector3Int> GetPath() {
      var result = new List<Vector3Int>();
      var currentIndex = 0;
      var currentTargetPosition = Position;
      while (currentIndex < _plannedMovement.Count) {
        currentTargetPosition += (Vector3Int)_plannedMovement[currentIndex].ToUnitVector();
        result.Add(currentTargetPosition);
        currentIndex++;
      }
      return result;
    }

    private IEnumerator OnEnemyTurnPreEnd() {
      PrePlanAction();
      yield break;
    }

    /// <summary>
    /// Movement strategy is to move in the shortest possible direction to make a
    /// straight line to the target, then move in that straight line.
    /// 
    /// Terrain isn't used, because we don't care about obstacles in the same way.
    /// </summary>
    private void PrePlanAction() {
      Debug.Log("Spirit planning its action");
      _plannedMovement = new();
      
      if (!TargetBones.TryGet(out var target)) {
        return;
      }
      
      var remainingMovement = _encounterState.GetResourceAmount(ExhaustibleResources.Instance.mp); 
      var targetPosition = target.Position;
      var xDiff = targetPosition.x - Position.x;
      var yDiff = targetPosition.y - Position.y;

      
      if (Math.Abs(xDiff) > Math.Abs(yDiff)) {
        remainingMovement = PlanMovementInAxis(yDiff, false, remainingMovement);
        PlanMovementInAxis(xDiff, true, remainingMovement);
      } else {
        remainingMovement = PlanMovementInAxis(xDiff, true, remainingMovement);
        PlanMovementInAxis(yDiff, false, remainingMovement);
      }
    }

    private int PlanMovementInAxis(int diff, bool isXAxis, int remainingMovement) {
      for (int i = 1; i <= Math.Abs(diff); i++) {
        if (remainingMovement == 0) {
          return 0;
        }

        var axisDirection = diff < 0 ? -1 : 1;
        var unitDirection = isXAxis ? new Vector2Int(axisDirection, 0) : new Vector2Int(0, axisDirection);
        _plannedMovement.Add(unitDirection.ToFacingDirection());
        remainingMovement--;
      }

      return remainingMovement;
    }
    
    private IEnumerator DissipateAndHandleBones(Bones bones) {
      _plannedMovement.Clear();
      var deathAnimationComplete = false;
      OneOffAnimation?.Invoke("death", () => deathAnimationComplete = true);
      yield return new WaitUntil(() => deathAnimationComplete);
      _renderer.enabled = false;
      yield return OnTargetBonesReached(bones);
      Destroy(gameObject);
    }

    private IEnumerator Dissipate() {
      _plannedMovement.Clear();
      var deathAnimationComplete = false;
      OneOffAnimation?.Invoke("death", () => deathAnimationComplete = true);
      yield return new WaitUntil(() => deathAnimationComplete);
      yield return new WaitForSeconds(1);
      _renderer.enabled = false;
      Destroy(gameObject);
    }

    public IEnumerator Push(FacingDirection direction) {
      yield return ExecuteMovement(new List<FacingDirection> { direction });
    }

    public Vector3Int GetPushTarget(Vector3Int sourceActor) {
      var direction = FacingUtilities.DirectionBetween(sourceActor, Position);
      return Position + (Vector3Int)direction.ToUnitVector();
    }

    private IEnumerator MoveBetween(Vector3Int currentPosition, Vector3Int targetPosition) {
      var worldPositionStart = GridUtils.CellCenterWorld(currentPosition);
      var worldPositionEnd = GridUtils.CellCenterWorld(targetPosition);

      var progressToNextNode = 0f;
      while (progressToNextNode < 1) {
        var position = Vector3.Lerp(worldPositionStart, worldPositionEnd, progressToNextNode);
        position.z = 1f;
        transform.position = position;
        progressToNextNode += (speedUnitsPerSec * Time.deltaTime);
        yield return null;
      }
      
      worldPositionEnd.z = 1f;
      transform.position = worldPositionEnd;
    }

    private void OnBonesCollected(Bones bones) {
      if (_targetBones.TryGet(out var targetBones) && targetBones == bones) {
        StartCoroutine(Dissipate());
      }
    }

    public DisplayDetails GetDisplayDetails() {
      var metadata = (EnemyUnitMetadata)_encounterState.metadata;
      return new DisplayDetails() {
          Name = metadata.displayName,
          AdditionalDetails = new() {
              "Seeks the dead",
              metadata.shortDescription,
          },
      };
    }

    protected virtual void OnPassThroughUnit(EncounterActor victim) {
      victim.ExpendResource(ExhaustibleResources.Instance.hp, damageOnCollision);
    }

    protected virtual IEnumerator OnTargetBonesReached(Bones bones) {
      yield return bones.ReviveUnit();
    }

    protected virtual IEnumerator OnPassOverTile(Vector3Int tile) {
      yield break;
    }
  }
}