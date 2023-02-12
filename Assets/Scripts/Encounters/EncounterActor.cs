using System;
using System.Collections.Generic;
using Encounters.Effects;
using RuntimeVars.Encounters.Events;
using State.Unit;
using Terrain;
using Units;
using Units.Abilities.AOE;
using UnityEngine;

namespace Encounters {
  public abstract class EncounterActor : MonoBehaviour, IPlacedOnGrid {
    [SerializeField] protected List<StatusEffect> activeStatusEffects;
    [SerializeField] protected EncounterEvents encounterEvents;
    
    private UnitMover _mover;
    public Vector3Int Position {
      get => EncounterState.position;
      set => EncounterState.position = value;
    }
    
    public abstract UnitEncounterState EncounterState { get; protected set; }

    protected virtual void Awake() {
      _mover = GetComponent<UnitMover>();
    }

    protected virtual void OnEnable() {
      encounterEvents.applyAoeEffect.RegisterListener(OnApplyAoeEffect);
    }

    protected virtual void OnDisable() {
      encounterEvents.applyAoeEffect.UnregisterListener(OnApplyAoeEffect);
    }

    protected virtual void Update() {
      UpdateStatusEffects();
    }
    
    private void UpdateStatusEffects() {
      for (int i = activeStatusEffects.Count - 1; i >= 0; i--) {
        if (activeStatusEffects[i].UpdateAndMaybeDestroy(this)) {
          activeStatusEffects.RemoveAt(i);
        }
      }
    }

    private void OnApplyAoeEffect(AreaOfEffect aoe, StatusEffect effect) {
      if (aoe.AffectsPoint(Position)) {
        AddStatusEffect(effect);
      }
    }

    public void MoveAlongPath(TravelPath path, Action callback) {
      if (!path.IsViable()) {
        callback();
        return;
      }
      _mover.ExecuteMovement(path.Path, callback);
    }

    public void AddStatusEffect(StatusEffect effectTemplate) {
      var effect = effectTemplate.Apply();
      activeStatusEffects.Add(effect);
    }
  }
}