using System;
using System.Collections.Generic;
using Encounters.Effects;
using Pathfinding;
using State.Unit;
using Units;
using UnityEngine;

namespace Encounters {
  public abstract class EncounterActor : MonoBehaviour {
    public List<StatusEffect> activeStatusEffects;
    
    private UnitMover _mover;
    public Vector3Int Position {
      get => EncounterState.position;
      set => EncounterState.position = value;
    }
    
    public abstract UnitEncounterState EncounterState { get; protected set; }

    protected virtual void Awake() {
      _mover = GetComponent<UnitMover>();
    }

    protected virtual void Update() {
      UpdateStatusEffects();
    }
    
    private void UpdateStatusEffects() {
      for (int i = activeStatusEffects.Count - 1; i >= 0; i--) {
        if (activeStatusEffects[i] == null) {
          activeStatusEffects.RemoveAt(i);
          continue;
        }
        
        activeStatusEffects[i].Update();
      }
    }

    public void MoveAlongPath(TravelPath path, Action callback) {
      _mover.ExecuteMovement(path.Path, callback);
    }

    public void AddStatusEffect(StatusEffect effect) {
      effect.Init(this);
      activeStatusEffects.Add(effect);
    }
  }
}