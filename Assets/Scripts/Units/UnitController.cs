﻿using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using State;
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

    private void Awake() {
      var grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      _placementManager = new UnitPlacementManager(grid, this, speedUnitsPerSec);
      _hpBar = transform.Find("UnitIndicators").GetComponentInChildren<Slider>();
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
    
    /// <returns>Whether the unit is eligible to move along the path</returns>
    public bool MoveAlongPath(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      if (!CouldMoveAlongPath(path)) {
        return false;
      }
      
      _placementManager!.ExecuteMovement(path, onCompleteCallback);
      return true;
    }

    public bool CouldMoveAlongPath(LinkedList<Vector3Int> path) {
      if (path == null) {
        return false;
      }
      var pathLength = path.Count;
      return pathLength > 0 && pathLength - 1 <= State.MovementRange;
    }
  }
}