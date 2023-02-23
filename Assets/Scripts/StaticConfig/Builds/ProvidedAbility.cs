using System;
using Units.Abilities;

namespace StaticConfig.Builds {
  [Serializable]
  public class ProvidedAbility {
    public UnitAbility ability;
    public int useRange = 1;
  }
}