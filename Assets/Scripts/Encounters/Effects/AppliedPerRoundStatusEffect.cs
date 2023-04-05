using Common.Events;
using Events;
using State.Unit;

namespace Encounters.Effects {
  public abstract class AppliedPerRoundStatusEffect : AppliedStatusEffect {
    private int _roundsUntilNextEnactment;
    private int _enactmentsUntilDeath;
    protected EncounterActor _victim;
    protected PerRoundStatusEffect _sourceEffect;

    private GameEvent NewRoundEvent => 
        _victim.EncounterState.faction == UnitFaction.Enemy
            ? Dispatch.Encounters.EnemyTurnStart : Dispatch.Encounters.PlayerTurnStart;

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
      OnInitialize();
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
    protected virtual void OnInitialize() { }
  }
}