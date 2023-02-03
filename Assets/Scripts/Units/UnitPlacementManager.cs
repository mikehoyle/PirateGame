using System;
using System.Collections.Generic;
using Common;
using Units.Rendering;
using UnityEngine;

namespace Units {
  /// <summary>
  /// Handles the placement of a unit in-world.
  /// </summary>
  public class UnitPlacementManager {
    private enum State {
      AtRest,
      InMotion,
    }
    
    private readonly Grid _grid;
    private readonly UnitController _unit;
    private readonly float _speedUnitsPerSec;
    private State _movementState;
    
    // Movement-specific vars
    private LinkedListNode<Vector3> _motionPath;
    private float _progressToNextNode;
    private Action _onMovementCompleteCallback;
    private readonly AnimatedCompositeSprite _sprite;


    public UnitPlacementManager(Grid grid, UnitController unit, AnimatedCompositeSprite sprite, float speedUnitsPerSec) {
      _grid = grid;
      _unit = unit;
      _sprite = sprite;
      _speedUnitsPerSec = speedUnitsPerSec;
      _movementState = State.AtRest;
    }

    public void Update() {
      if (_movementState == State.InMotion) {
        if (_motionPath.Next == null) {
          OnMovementComplete();
          return;
        }

        PlayMovementAnimation();
        
        _progressToNextNode += (_speedUnitsPerSec * Time.deltaTime);
        if (_progressToNextNode >= 1) {
          _motionPath = _motionPath.Next;
          _progressToNextNode -= 1;
        }
      }
    }
    private void PlayMovementAnimation() {
      var direction = _motionPath.Next!.Value - _motionPath.Value;
      if (Floats.GreaterThanZero(direction.x)) {
        if (Floats.GreaterThanZero(direction.y)) {
          _sprite.Play(CompositeAnimation.Type.WalkNe);
        } else {
          _sprite.Play(CompositeAnimation.Type.WalkSe);
        }
      } else {
        if (Floats.GreaterThanZero(direction.y)) {
          _sprite.Play(CompositeAnimation.Type.WalkNw);
        } else {
          _sprite.Play(CompositeAnimation.Type.WalkSw);
        }
      }
    }

    public void ExecuteMovement(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      _movementState = State.InMotion;
      _progressToNextNode = 0;
      _onMovementCompleteCallback = onCompleteCallback;
      
      // Mark unit as in new position immediately.
      _unit.EncounterMetadata.Position = path.Last.Value;
      
      // Convert grid path to world path
      var worldPath = new LinkedList<Vector3>();
      foreach (var gridCell in path) {
        worldPath.AddLast(_grid.GetCellCenterWorld(gridCell));
      }
      _motionPath = worldPath.First;
    }
    
    public Vector3 GetPlacement() {
      if (_movementState == State.InMotion) {
        if (_motionPath.Next == null) {
          OnMovementComplete();
          return GetPlacement();
        }
        
        return Vector3.Lerp(_motionPath.Value, _motionPath.Next.Value, _progressToNextNode);
      }
      
      return _grid.GetCellCenterWorld(_unit.EncounterMetadata.Position);
    }

    private void OnMovementComplete() {
      _movementState = State.AtRest;
      _sprite.Play(CompositeAnimation.Type.IdleSw);
      _onMovementCompleteCallback();
      
      _motionPath = null;
      _onMovementCompleteCallback = null;
      _progressToNextNode = 0;
    }
  }
}