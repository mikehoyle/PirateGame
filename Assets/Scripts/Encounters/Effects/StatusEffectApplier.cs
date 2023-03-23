using System.Collections.Generic;
using State.Unit;
using Units.Abilities;

namespace Encounters.Effects {
  public class StatusEffectApplier {
    private readonly StatusEffect _effect;
    private readonly List<UnitFaction> _affectedFactions;
    private readonly float _skillTestResult;
    private readonly EncounterActor _actor;

    public StatusEffectApplier(
        StatusEffect effect,
        UnitAbility.AbilityExecutionContext context,
        List<UnitFaction> affectedFactions,
        float skillTestResult) {
      _effect = effect;
      _actor = context.Actor;
      _affectedFactions = affectedFactions;
      _skillTestResult = skillTestResult;
    }

    public void ApplyTo(EncounterActor victim) {
      if (_affectedFactions.Contains(victim.EncounterState.faction)) {
        var instance = _effect.ApplyTo(victim);
        instance.PreCalculateEffect(_actor, _skillTestResult);
      }
    }
  }
}