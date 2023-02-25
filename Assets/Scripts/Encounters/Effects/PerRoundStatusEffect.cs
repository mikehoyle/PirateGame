using System;
using Common.Events;
using RuntimeVars.Encounters.Events;
using State.Unit;
using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  [Serializable]
  public abstract class PerRoundStatusEffect : StatusEffect {
    [SerializeField] protected EncounterEvents encounterEvents;
    public int numRoundsPerEffectEnactment = 1;
    public int numEnactmentsBeforeDeath = -1;
    public bool enactOnceOnApplication;
    public UnitFaction unitFaction;
    
    public override string DisplayString() {
      return "Per-round status effect";
    }

    protected EmptyGameEvent NewRoundEvent() {
      if (unitFaction == UnitFaction.Enemy) {
        return encounterEvents.enemyTurnStart;
      }
      return encounterEvents.playerTurnStart;
    }

    public abstract class PerRoundStatusEffectInstance : IStatusEffectInstance {
      private int _roundsUntilNextEnactment;
      private int _enactmentsUntilDeath;
      private bool _isDestroyed = false;
      protected readonly EncounterActor _victim;
      protected readonly PerRoundStatusEffect _sourceEffect;

      protected PerRoundStatusEffectInstance(
          PerRoundStatusEffect sourceEffect,
          EncounterActor victim) {
        _sourceEffect = sourceEffect;
        _victim = victim;
        _roundsUntilNextEnactment = sourceEffect.numRoundsPerEffectEnactment;
        _enactmentsUntilDeath = sourceEffect.numEnactmentsBeforeDeath < 1 ?
            int.MaxValue : sourceEffect.numEnactmentsBeforeDeath;
        _isDestroyed = false;
        sourceEffect.NewRoundEvent().RegisterListener(OnNewRound);
        if (sourceEffect.enactOnceOnApplication) {
          EnactEffect(victim);
        }
      }
      
      
      public bool UpdateAndMaybeDestroy(EncounterActor victim) {
        // This is necessary to get removed from the actor's status effect list (and ultimately get GC'ed).
        // Janky design but here we are.
        return _isDestroyed;
      }

      public abstract void EnactEffect(EncounterActor victim);

      private void OnNewRound() {
        if (_isDestroyed) {
          return;
        }

        _roundsUntilNextEnactment--;
        if (_roundsUntilNextEnactment <= 0) {
          EnactEffect(_victim);
          _roundsUntilNextEnactment = _sourceEffect.numRoundsPerEffectEnactment;
        }

        _enactmentsUntilDeath--;
        if (_enactmentsUntilDeath <= 0) {
          DestroySelf();
        }
      }

      private void DestroySelf() {
        // Failsafe just in case this object sticks around longer than the Unity object's lifecycle
        _isDestroyed = true;
        OnDestroy();
      }
      
      private void OnDestroy() {
        _sourceEffect.NewRoundEvent().UnregisterListener(OnNewRound);
      }
    }
  }
}