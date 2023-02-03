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

    public UnitEncounterMetadata EncounterMetadata;
    private AnimatedCompositeSprite _sprite;

    private void Awake() {
      var grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      _sprite = GetComponentInChildren<AnimatedCompositeSprite>();
      _placementManager = new UnitPlacementManager(grid, this, _sprite, speedUnitsPerSec);
      _hpBar = transform.Find("UnitIndicators").GetComponentInChildren<Slider>();
    }

    private void Start() {
      _sprite.SetColorForFaction(State.Faction);
    }

    private void Update() {
      _placementManager!.Update();
      _hpBar.value = EncounterMetadata.CurrentHp;
      transform.position = WorldPosition;
    }

    public void Init(UnitState state) {
      Init(state, Vector3Int.zero);
    }

    public void Init(UnitState state, Vector3Int positionOffset) {
      State = state;
      _hpBar.maxValue = State.MaxHp;
      _hpBar.minValue = 0;
      _hpBar.value = State.MaxHp;
      EncounterMetadata = new UnitEncounterMetadata(this, State.StartingPosition + positionOffset);
      transform.position = WorldPosition;
    }
    
    /// <returns>Whether the unit is eligible to move along the path</returns>
    public bool MoveAlongPath(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      if (!CouldMoveAlongPath(path)) {
        return false;
      }
      
      _placementManager!.ExecuteMovement(path, () => {
        onCompleteCallback();
      });
      EncounterMetadata.RemainingMovement -= (path.Count - 1);
      return true;
    }

    public bool CouldMoveAlongPath(LinkedList<Vector3Int> path) {
      if (path == null) {
        return false;
      }
      var pathLength = path.Count;
      return pathLength > 0 && pathLength - 1 <= EncounterMetadata.RemainingMovement;
    }

    public bool IsUnitEnemy(UnitController unit) {
      return State.Faction != unit.State.Faction;
    }
  }
}