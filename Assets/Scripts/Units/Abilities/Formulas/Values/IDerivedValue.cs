using Encounters;

namespace Units.Abilities.Formulas.Values {
  public interface IDerivedValue {
    public float GetValue(EncounterActor actor, float skillTestResult);
    public string DisplayString();
  }
}
