using System;
using Units.Abilities;

namespace StaticConfig.Builds {
  [Serializable]
  public class ProvidedAbility {
    public UnitAbility ability;
    public int useRange = 1;

    public string DisplayString() {
      return $"{ability.displayString} (Use range: {useRange})";
    }
  }
}