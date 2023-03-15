using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Animation;
using Common.Grid;
using HUD.Encounter.HoverDetails;
using Optional;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units;
using UnityEngine;

namespace Encounters.Enemies {
  public class SpiritUnitController : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider, IDirectionalAnimatable {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private ExhaustibleResources resources;
    [SerializeField] private SpiritCollection spiritsInEncounter;
    [SerializeField] private int damageOnCollision = 2;
    [SerializeField] private float speedUnitsPerSec;
    
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

    private void Awake() {
      _plannedMovement = new();
      _renderer = GetComponent<SpriteRenderer>();
      AnimationState = "idle";
    }

    private void OnEnable() {
      spiritsInEncounter.spirits.Add(this);
      encounterEvents.enemyTurnPreEnd.RegisterListener(OnEnemyTurnPreEnd);
    }

    private void OnDisable() {
      spiritsInEncounter.spirits.Remove(this);
      encounterEvents.enemyTurnPreEnd.UnregisterListener(OnEnemyTurnPreEnd);
    }
    
    public void Init(UnitEncounterState encounterState) {
      _encounterState = encounterState;
      transform.position = GridUtils.CellCenterWorldStatic(encounterState.position);
    }

    public IEnumerator ExecuteMovementPlan() {
      if (_plannedMovement.Count == 0) {
        yield break;
      }

      var currentPosition = Position;
      var currentIndex = 0;
      while (currentIndex < _plannedMovement.Count) {
        FacingDirection = _plannedMovement[currentIndex];
        var targetPosition = currentPosition + (Vector3Int)_plannedMovement[currentIndex].ToUnitVector();

        if (SceneTerrain.TryGetComponentAtTile<EncounterActor>(targetPosition, out var actor)) {
          actor.ExpendResource(resources.hp, damageOnCollision);
          if (currentIndex == _plannedMovement.Count - 1) {
            // Never stop inside an occupied space. If we would, just keep going until we're not.
            _plannedMovement.Add(_plannedMovement[currentIndex]);
          }
        }

        yield return MoveBetween(currentPosition, targetPosition);
        currentPosition = targetPosition;
        
        if (SceneTerrain.TryGetComponentAtTile<Bones>(targetPosition, out var bones)) {
          yield return CollectAndDissipate(bones);
          yield break;
        }
        
        currentIndex++;
      }
      
      Position = currentPosition;
      _plannedMovement.Clear();
    }

    public List<Vector3Int> GetPath() {
      var result = new List<Vector3Int>();
      var currentIndex = 0;
      var currentTargetPosition = Position;
      while (currentIndex < _plannedMovement.Count) {
        currentTargetPosition = currentTargetPosition + (Vector3Int)_plannedMovement[currentIndex].ToUnitVector();
        result.Add(currentTargetPosition);
        currentIndex++;
      }
      return result;
    }

    private void OnEnemyTurnPreEnd() {
      PrePlanAction();
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
      
      var remainingMovement = _encounterState.GetResourceAmount(resources.mp); 
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

    // TODO(P0): Properly handle if we collect someone else's target bones.
    private IEnumerator CollectAndDissipate(Bones bones) {
      _plannedMovement.Clear();
      OneOffAnimation?.Invoke("death");
      // TODO(P0): make this actually wait until animation end.. also probably handle
      //    one shot animations completely differently.
      yield return new WaitForSeconds(1);
      _renderer.enabled = false;
      yield return bones.ReviveUnit();
      Destroy(gameObject);
    }

    public IEnumerator Push(FacingDirection direction) {
      var targetPosition = Position + (Vector3Int)direction.ToUnitVector();
      if (!CouldMoveTo(targetPosition)) {
        yield break;
      }
      yield return MoveBetween(Position, targetPosition);
      Position = targetPosition;
    }

    public Vector3Int GetPushTarget(Vector3Int sourceActor) {
      var direction = FacingUtilities.DirectionBetween(sourceActor, Position);
      var targetPosition = Position + (Vector3Int)direction.ToUnitVector();
      return CouldMoveTo(targetPosition) ? targetPosition : Position;
    }

    private bool CouldMoveTo(Vector3Int targetPosition) {
      if (SceneTerrain.IsTileOccupied(targetPosition)
          && !SceneTerrain.TryGetComponentAtTile<Bones>(targetPosition, out _)) {
        // Cannot be pushed onto an occupied tile, but can be pushed onto bones
        return false;
      }
      // To future self-criticising eyes, it's much clearer to format this way instead of one giant conditional.
      return true;
    }

    private IEnumerator MoveBetween(Vector3Int currentPosition, Vector3Int targetPosition) {
      var worldPositionStart = GridUtils.CellCenterWorldStatic(currentPosition);
      var worldPositionEnd = GridUtils.CellCenterWorldStatic(targetPosition);

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

    public DisplayDetails GetDisplayDetails() {
      new WaitForSeconds(1);
      return new DisplayDetails() {
          Name = "Spirit",
          AdditionalDetails = new() {
              "Seeks the dead",
          },
      };
    }
  }
}