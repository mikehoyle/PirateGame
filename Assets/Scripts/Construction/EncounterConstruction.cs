using Common;
using Encounters;
using Encounters.AbilityProviders;
using Optional;
using Units.Abilities;

namespace Construction {
  public class EncounterConstruction : InGameConstruction, IAbilityProvider {
    public Option<UnitAbility> TryProvideAbility(EncounterActor actor) {
      if (Metadata.providedAbility.ability == null) {
        return Option.None<UnitAbility>();
      }
      
      var distance = GridUtils.DistanceBetween(Position, actor.Position);
      if (distance > Metadata.providedAbility.useRange) {
        return Option.None<UnitAbility>();
      }
      
      return Option.Some(Metadata.providedAbility.ability);
    }
  }
}