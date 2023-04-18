using System.Collections.Generic;
using Common;
using Optional;
using State.Unit;

namespace Encounters.Effects {
  public class StatusEffectApplier {
    private readonly StatusEffect _effect;
    private readonly List<UnitFaction> _affectedFactions;
    private readonly float _skillTestResult;
    private readonly Option<EncounterActor> _actor;

    public StatusEffectApplier(
        StatusEffect effect,
        List<UnitFaction> affectedFactions) {
      _effect = effect;
      _actor = Option.None<EncounterActor>();
      _affectedFactions = affectedFactions;
      _skillTestResult = 1;
    }
    
    public StatusEffectApplier(
        StatusEffect effect,
        Option<EncounterActor> actor,
        List<UnitFaction> affectedFactions,
        float skillTestResult) {
      _effect = effect;
      _actor = actor;
      _affectedFactions = affectedFactions;
      _skillTestResult = skillTestResult;
    }

    public void ApplyTo(EncounterActor victim) {
      if (_affectedFactions.Contains(victim.EncounterState.faction)) {
        var instance = _effect.ApplyTo(victim);
        if (_actor.TryGet(out var actor)) {
          instance.PreCalculateEffect(actor, _skillTestResult);
        } else {
          instance.PreCalculateNoContext();
        }
      }
    }
  }
}