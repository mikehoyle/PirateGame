using System;
using Units.Abilities;

namespace Encounters.Effects {
  [Serializable]
  public abstract class StatusEffect {
    public abstract IStatusEffectInstance NewInstance(EncounterActor victim);
    public abstract string DisplayString();
  }
}