using System;

namespace Encounters.Effects {
  [Serializable]
  public abstract class StatusEffect {
    public abstract AppliedStatusEffect ApplyTo(EncounterActor victim);
    public abstract string DisplayString();
  }
}