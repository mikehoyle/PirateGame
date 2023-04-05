using StaticConfig.Units;

namespace Encounters.Effects {
  public interface IStatModifier {
    int GetStatModifier(EncounterActor actor, Stat stat);
  }
}