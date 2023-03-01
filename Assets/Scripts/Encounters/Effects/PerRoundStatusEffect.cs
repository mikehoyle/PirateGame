using System;
using RuntimeVars.Encounters.Events;

namespace Encounters.Effects {
  [Serializable]
  public abstract class PerRoundStatusEffect : StatusEffect {
    public EncounterEvents encounterEvents;
    public int numRoundsPerEffectEnactment = 1;
    public int numEnactmentsBeforeDeath = -1;
    public bool enactOnceOnApplication;
    
    public override string DisplayString() {
      return "Per-round status effect";
    }
  }
}