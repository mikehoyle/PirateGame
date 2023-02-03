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
    
    [CanBeNull] private UnitPlacementManager _placementManager;
    public Vector3Int Position { get; set; }
    public UnitState State { get; private set; }
    // TODO(P1): This doesn't seem to be the true center.
    public Vector3 WorldPosition => _placementManager!.GetPlacement();
    private AnimatedCompositeSprite _sprite;

    private void Awake() {
      var grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      _sprite = GetComponentInChildren<AnimatedCompositeSprite>();
      _placementManager = new UnitPlacementManager(grid, this, _sprite, speedUnitsPerSec);
    }

    private void Start() {
      _sprite.SetColorForFaction(State.Faction);
    }

    private void Update() {
      _placementManager!.Update();
      transform.position = WorldPosition;
    }

    public void Init(UnitState state) {
      Init(state, Vector3Int.zero);
    }

    public void Init(UnitState state, Vector3Int positionOffset) {
      State = state;
      Position = State.StartingPosition + positionOffset;
      transform.position = WorldPosition;
    }
    
    /// <returns>Whether the unit is eligible to move along the path</returns>
    public bool MoveAlongPath(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      if (!IsPathViable(path)) {
        return false;
      }
      
      _placementManager!.ExecuteMovement(path, onCompleteCallback);
      return true;
    }

    public static bool IsPathViable(LinkedList<Vector3Int> path) {
      return path != null && path.Count > 1;
    }
  }
}