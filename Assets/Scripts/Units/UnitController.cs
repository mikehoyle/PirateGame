using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using State;
using Units.Rendering;
using UnityEngine;
using UnityEngine.UI;

namespace Units {
  public class UnitController : MonoBehaviour {
    [SerializeField] private float speedUnitsPerSec;
    
    private Slider _hpBar;
    [CanBeNull] private UnitPlacementManager _placementManager;

    public UnitState State { get; private set; }
    // TODO(P1): This doesn't seem to be the true center.
    public Vector3 WorldPosition => _placementManager!.GetPlacement();
    
    public List<UnitAction> CapableActions { get; } = new();
    public List<UnitAction> AvailableActions { get; private set; } = new();
    public int RemainingMovement { get; private set; }

    private void Awake() {
      var grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      var sprite = GetComponentInChildren<AnimatedCompositeSprite>();
      _placementManager = new UnitPlacementManager(grid, this, sprite, speedUnitsPerSec);
      _hpBar = transform.Find("UnitIndicators").GetComponentInChildren<Slider>();
      AddAvailableActions();
    }
    
    private void AddAvailableActions() {
      CapableActions.Add(UnitAction.Move);
      CapableActions.Add(UnitAction.AttackMelee);
      CapableActions.Add(UnitAction.EndTurn);
    }

    private void Update() {
      _placementManager!.Update();
      _hpBar.value = State.CurrentHp;
      transform.position = WorldPosition;
    }

    public void Init(UnitState state) {
      State = state;
      _hpBar.maxValue = State.MaxHp;
      _hpBar.minValue = 0;
      _hpBar.value = State.CurrentHp;

      transform.position = WorldPosition;
    }

    public void ActivateTurn() {
      RemainingMovement = State.MovementRange;
      AvailableActions = new(CapableActions);
    }
    
    /// <returns>Whether the unit is eligible to move along the path</returns>
    public bool MoveAlongPath(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      if (!CouldMoveAlongPath(path)) {
        return false;
      }
      
      _placementManager!.ExecuteMovement(path, () => {
        onCompleteCallback();
      });
      RemainingMovement -= (path.Count - 1);
      return true;
    }

    public bool CouldMoveAlongPath(LinkedList<Vector3Int> path) {
      if (path == null) {
        return false;
      }
      var pathLength = path.Count;
      return pathLength > 0 && pathLength - 1 <= RemainingMovement;
    }

    public bool IsUnitEnemy(UnitController unit) {
      return State.Faction != unit.State.Faction;
    }
  }
}