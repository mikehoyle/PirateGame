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
using Units;
using UnityEngine;

namespace Encounters.Enemies {
  public class SpiritUnitController : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider, IDirectionalAnimatable {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private ExhaustibleResources resources;
    [SerializeField] private SpiritCollection spiritsInEncounter;
    
    private UnitEncounterState _encounterState;
    private SpiritMover _mover;
    private Option<Bones> _targetBones;
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
      _mover = GetComponent<SpiritMover>();
      _plannedMovement = new();
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

    public Coroutine ExecuteMovementPlan() {
      return StartCoroutine(_mover.ExecuteMovement(_plannedMovement));
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

    public IEnumerator Dissipate() {
      OneOffAnimation?.Invoke("death");
      // TODO(P0): make this actually wait until animation end.. also probably handle
      //    one shot animations completely differently.
      yield return new WaitForSeconds(1);
      Destroy(gameObject);
    }

    public DisplayDetails GetDisplayDetails() {
      return new DisplayDetails() {
          Name = "Spirit",
          AdditionalDetails = new() {
              "Seeks the dead",
          },
      };
    }
  }
}