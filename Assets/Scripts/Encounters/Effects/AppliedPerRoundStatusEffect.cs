using System;
using Common.Events;
using State.Unit;

namespace Encounters.Effects {
  public abstract class AppliedPerRoundStatusEffect : AppliedStatusEffect {
    private int _roundsUntilNextEnactment;
    private int _enactmentsUntilDeath;
    protected EncounterActor _victim;
    protected PerRoundStatusEffect _sourceEffect;

    private EmptyGameEvent NewRoundEvent => 
        _victim.EncounterState.faction == UnitFaction.Enemy
            ? _sourceEffect.encounterEvents.enemyTurnStart
            : _sourceEffect.encounterEvents.playerTurnStart;

    public void Initialize(PerRoundStatusEffect sourceEffect, EncounterActor victim) {
      _sourceEffect = sourceEffect;
      _victim = victim;
      _roundsUntilNextEnactment = sourceEffect.numRoundsPerEffectEnactment;
      _enactmentsUntilDeath = sourceEffect.numEnactmentsBeforeDeath < 1 ?
          int.MaxValue : sourceEffect.numEnactmentsBeforeDeath;
      NewRoundEvent.RegisterListener(OnNewRound);
      if (sourceEffect.enactOnceOnApplication) {
        EnactEffect();
      }
    }

    private void OnDisable() {
      NewRoundEvent.UnregisterListener(OnNewRound);
    }

    private void OnNewRound() {
      _roundsUntilNextEnactment--;
      if (_roundsUntilNextEnactment <= 0) {
        EnactEffect();
        _roundsUntilNextEnactment = _sourceEffect.numRoundsPerEffectEnactment;
      }

      _enactmentsUntilDeath--;
      if (_enactmentsUntilDeath <= 0) {
        Destroy(this);
      }
    }

    protected abstract void EnactEffect();
  }
}