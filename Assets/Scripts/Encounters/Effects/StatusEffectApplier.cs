using Units.Abilities;

namespace Encounters.Effects {
  public class StatusEffectApplier {
    private readonly StatusEffect _effect;
    private readonly UnitAbility.AbilityExecutionContext _context;
    private readonly float _skillTestResult;
    
    public StatusEffectApplier(
        StatusEffect effect,
        UnitAbility.AbilityExecutionContext context,
        float skillTestResult) {
      _effect = effect;
      _context = context;
      _skillTestResult = skillTestResult;
    }

    public void ApplyTo(EncounterActor victim) {
      var instance = _effect.ApplyTo(victim);
      instance.PreCalculateEffect(_context, _skillTestResult);
    }
  }
}