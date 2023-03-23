using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Animation;
using Common.Grid;
using Encounters;
using Encounters.Obstacles;
using FMODUnity;
using MilkShake;
using Optional;
using StaticConfig.Units;
using Terrain;
using UnityEngine;

namespace Units {
  public class UnitMover : MonoBehaviour {
    [SerializeField] private float speedUnitsPerSec;
    [SerializeField] private int dropInHeight;
    [SerializeField] private float dropInGravity;
    [SerializeField] private ShakePreset dropInShake;
    [SerializeField] private EventReference dropInImpactSound;
    
    private EncounterActor _unit;
    private Option<StudioEventEmitter> _footstepsSound;

    private void Awake() {
      _unit = GetComponent<EncounterActor>();
      _footstepsSound = GetComponent<StudioEventEmitter>().SomeNotNull();
    }

    private void Start() {
      _unit.AnimationState = AnimationNames.Idle;
      transform.position = GridUtils.CellCenterWorld(_unit.Position);
    }

    public IEnumerator ExecuteMovement(LinkedList<Vector3Int> path, bool expendMp) {
      var progressToNextNode = 0f;
      var currentPosition = path.First;
      _footstepsSound.MatchSome(sound => sound.Play());
      while (currentPosition.Next != null) {
        if (expendMp && _unit.CurrentMovementRange() < 1) {
          break;
        }
        
        var currentNodeWorld = GridUtils.CellCenterWorld(currentPosition.Value);
        var nextNodeWorld = GridUtils.CellCenterWorld(currentPosition.Next.Value);
        SetFacingDirection(currentNodeWorld, nextNodeWorld);
        _unit.AnimationState = AnimationNames.Walk;
        
        transform.position = Vector3.Lerp(currentNodeWorld, nextNodeWorld, progressToNextNode);
        progressToNextNode += (speedUnitsPerSec * Time.deltaTime);
        
        if (progressToNextNode >= 1) {
          currentPosition = currentPosition.Next;
          _unit.Position = currentPosition.Value;
          progressToNextNode -= 1;

          if (expendMp) {
            _unit.ExpendResource(ExhaustibleResources.Instance.mp, 1);  
          }

          // Every step along the way we want to check if something has prevented our movement
          // or if we picked up a status effect.
          foreach (var tileOccupant in SceneTerrain.GetAllTileOccupants(currentPosition.Value)) {
            if (tileOccupant.TryGetComponent<PlacedObject>(out var placedObject)) {
              placedObject.HandleWalkOver(_unit);
            }
          }
        }
        yield return null;
      }
      
      OnMovementComplete();
    }
    
    private void SetFacingDirection(Vector3 source, Vector3 destination) {
      var direction = destination - source;
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

    public IEnumerator DropIn(Action onCompleteCallback) {
      _unit.EnableShadow(false);
      var currentPosition = GridUtils.CellCenterWorld(_unit.Position);
      var destinationY = currentPosition.y;
      currentPosition.y += dropInHeight;
      var currentVelocityY = 3f;

      while (currentPosition.y > destinationY) {
        currentVelocityY += (dropInGravity * Time.deltaTime);
        currentPosition.y -= (currentVelocityY * Time.deltaTime);
        transform.position = currentPosition;
        yield return null;
      }
      
      _unit.EnableShadow(true);
      Shaker.ShakeAll(dropInShake);
      RuntimeManager.PlayOneShot(dropInImpactSound);
      OnMovementComplete();
      onCompleteCallback();
    }

    public void SnapToPosition(Vector3Int position) {
      _unit.Position = position;
      transform.position = GridUtils.CellCenterWorld(position);
    }

    private void OnMovementComplete() {
      _footstepsSound.MatchSome(sound => sound.Stop());
      _unit.AnimationState = AnimationNames.Idle;
      transform.position = GridUtils.CellCenterWorld(_unit.Position);
    }
  }
}