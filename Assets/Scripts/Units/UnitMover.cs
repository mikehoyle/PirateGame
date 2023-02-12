﻿using System;
using System.Collections.Generic;
using Common;
using Encounters;
using State.Unit;
using Terrain;
using UnityEngine;

namespace Units {
  public class UnitMover : MonoBehaviour {
    [SerializeField] private float speedUnitsPerSec;
    
    private Grid _grid;
    private bool _currentlyMoving;
    private EncounterActor _unit;
    
    // Movement-specific vars
    private LinkedListNode<Vector3> _motionPath;
    private float _progressToNextNode;
    private Action _onMovementCompleteCallback;

    private void Awake() {
      _grid = SceneTerrain.Get().Grid;
      _unit = GetComponent<EncounterActor>();
      _currentlyMoving = false;
    }
    
    public void Update() {
      if (_currentlyMoving) {
        if (_motionPath.Next == null) {
          OnMovementComplete();
          return;
        }

        SetFacingDirection();
        transform.position = Vector3.Lerp(_motionPath.Value, _motionPath.Next!.Value, _progressToNextNode);
        
        _progressToNextNode += (speedUnitsPerSec * Time.deltaTime);
        if (_progressToNextNode >= 1) {
          _motionPath = _motionPath.Next;
          _progressToNextNode -= 1;
        }
        return;
      }
      
      transform.position = _grid.GetCellCenterWorld(_unit.Position);
    }
    
    private void SetFacingDirection() {
      var direction = _motionPath.Next!.Value - _motionPath.Value;
      if (Floats.GreaterThanZero(direction.x)) {
        if (Floats.GreaterThanZero(direction.y)) {
          _unit.EncounterState.facingDirection = FacingDirection.NorthEast;
        } else {
          _unit.EncounterState.facingDirection = FacingDirection.SouthEast;
        }
      } else {
        if (Floats.GreaterThanZero(direction.y)) {
          _unit.EncounterState.facingDirection = FacingDirection.NorthWest;
        } else {
          _unit.EncounterState.facingDirection = FacingDirection.SouthWest;
        }
      }
    }

    public void ExecuteMovement(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      _currentlyMoving = true;
      _progressToNextNode = 0;
      _onMovementCompleteCallback = onCompleteCallback;
      
      // Mark unit as in new position immediately.
      _unit.Position = path.Last.Value;
      
      // Convert grid path to world path
      var worldPath = new LinkedList<Vector3>();
      foreach (var gridCell in path) {
        worldPath.AddLast(_grid.GetCellCenterWorld(gridCell));
      }
      _motionPath = worldPath.First;
    }

    private void OnMovementComplete() {
      _currentlyMoving = false;
      transform.position = _grid.GetCellCenterWorld(_unit.Position);
      _onMovementCompleteCallback();
      
      _motionPath = null;
      _onMovementCompleteCallback = null;
      _progressToNextNode = 0;
    }
  }
}