using System;
using Common.Events;
using RuntimeVars.Encounters.Events;
using State.Unit;
using UnityEngine;
using UnityEngine.Serialization;

namespace Encounters.Effects {
  [CreateAssetMenu(menuName = "Effects/PerRoundStatusEffect")]
  public class PerRoundStatusEffect : StatusEffect {
    [SerializeField] private EncounterEvents encounterEvents;
    public int numRoundsPerEffectEnactment = 1;
    public int numEnactmentsBeforeDeath = -1;
    public bool enactOnceOnApplication;
    public UnitFaction unitFaction;

    private int _roundsUntilNextEnactment;
    private int _enactmentsUntilDeath;
    private EncounterActor _victim;
    private bool _isDestroyed = false;

    public override void OnApply(EncounterActor victim) {
      if (enactOnceOnApplication) {
        EnactEffect(victim);
      }

      _victim = victim;
      _roundsUntilNextEnactment = numRoundsPerEffectEnactment;
      _enactmentsUntilDeath = numEnactmentsBeforeDeath < 1 ? int.MaxValue : numEnactmentsBeforeDeath;
      _isDestroyed = false;
      NewRoundEvent().RegisterListener(OnNewRound);
    }

    private void OnDestroy() {
      NewRoundEvent().UnregisterListener(OnNewRound);
    }

    public override bool UpdateAndMaybeDestroy(EncounterActor victim) {
      // This is necessary to get removed from the actor's status effect list (and ultimately get GC'ed).
      // Janky design but here we are.
      return _isDestroyed;
    }

    private void OnNewRound() {
      if (_isDestroyed) {
        return;
      }

      _roundsUntilNextEnactment--;
      if (_roundsUntilNextEnactment <= 0) {
        EnactEffect(_victim);
        _roundsUntilNextEnactment = numRoundsPerEffectEnactment;
      }

      _enactmentsUntilDeath--;
      if (_enactmentsUntilDeath <= 0) {
        DestroySelf();
      }
    }

    private void DestroySelf() {
      // Failsafe just in case this object sticks around longer than the Unity object's lifecycle
      _isDestroyed = true;
      Destroy(this);
      OnDestroy();
    }

    private EmptyGameEvent NewRoundEvent() {
      if (unitFaction == UnitFaction.Enemy) {
        return encounterEvents.playerTurnStart;
      }
      return encounterEvents.enemyTurnStart;
    }
  }
}