using Units.Abilities;

namespace Encounters.Effects {
  public class StatusEffectInstanceFactory {
    private readonly StatusEffect _effect;
    private readonly UnitAbility.AbilityExecutionContext _context;
    private readonly float _skillTestResult;
    
    public StatusEffectInstanceFactory(
        StatusEffect effect,
        UnitAbility.AbilityExecutionContext context,
        float skillTestResult) {
      _effect = effect;
      _context = context;
      _skillTestResult = skillTestResult;
    }
    
    public IStatusEffectInstance GetInstance(EncounterActor victim) {
      var instance = _effect.NewInstance(victim);
      instance.PreCalculateEffect(_context, _skillTestResult);
      return instance;
    }
  }
}