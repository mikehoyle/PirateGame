using Optional;
using Units.Abilities;

namespace Encounters.AbilityProviders {
  /// <summary>
  /// An interface to be applied to Components of GameObjects that can provide an ability
  /// to a unit, such as a ship cannon.
  /// </summary>
  public interface IAbilityProvider {
    Option<UnitAbility> TryProvideAbility(EncounterActor actor);
  }
}