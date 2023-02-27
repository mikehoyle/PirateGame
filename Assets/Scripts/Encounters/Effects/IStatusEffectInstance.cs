using Units.Abilities;

namespace Encounters.Effects {
  public interface IStatusEffectInstance {
    public void PreCalculateEffect(UnitAbility.AbilityExecutionContext context, float skillTestResult) { }
    /// <returns>If the effect was destroyed</returns>
    bool UpdateAndMaybeDestroy(EncounterActor victim);
    void EnactEffect(EncounterActor victim);
  }
}