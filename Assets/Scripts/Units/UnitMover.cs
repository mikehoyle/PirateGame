using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Common.Animation;
using Encounters;
using FMODUnity;
using MilkShake;
using Optional;
using Terrain;
using UnityEngine;

namespace Units {
  public class UnitMover : MonoBehaviour {
    [SerializeField] private float speedUnitsPerSec;
    [SerializeField] private int dropInHeight;
    [SerializeField] private float dropInGravity;
    [SerializeField] private ShakePreset dropInShake;
    [SerializeField] private EventReference dropInImpactSound;
    
    private SceneTerrain _terrain;
    private EncounterActor _unit;
    private Option<StudioEventEmitter> _footstepsSound;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _unit = GetComponent<EncounterActor>();
      _footstepsSound = GetComponent<StudioEventEmitter>().SomeNotNull();
    }

    private void Start() {
      _unit.AnimationState = AnimationNames.Idle;
      transform.position = _terrain.CellCenterWorld(_unit.Position);
    }

    public IEnumerator ExecuteMovement(LinkedList<Vector3Int> path) {
      var progressToNextNode = 0f;

      // Mark unit as in new position immediately.
      _unit.Position = path.Last.Value;
      
      // Convert grid path to world path
      var worldPath = new LinkedList<Vector3>();
      foreach (var gridCell in path) {
        worldPath.AddLast(_terrain.CellCenterWorld(gridCell));
      }
      var motionPath = worldPath.First;
      _footstepsSound.MatchSome(sound => sound.Play());
      while (motionPath.Next != null) {
        SetFacingDirection(motionPath.Value, motionPath.Next.Value);
        _unit.AnimationState = AnimationNames.Walk;
        transform.position = Vector3.Lerp(motionPath.Value, motionPath.Next.Value, progressToNextNode);
        progressToNextNode += (speedUnitsPerSec * Time.deltaTime);
        
        if (progressToNextNode >= 1) {
          motionPath = motionPath.Next;
          progressToNextNode -= 1;
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
      var currentPosition = _terrain.CellCenterWorld(_unit.Position);
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
      transform.position = _terrain.CellCenterWorld(position);
    }

    private void OnMovementComplete() {
      _footstepsSound.MatchSome(sound => sound.Stop());
      _unit.AnimationState = AnimationNames.Idle;
      transform.position = _terrain.CellCenterWorld(_unit.Position);
    }
  }
}