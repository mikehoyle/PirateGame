using UnityEngine;

namespace Encounters.Effects {
  public abstract class AppliedStatusEffect : MonoBehaviour {
    public virtual void PreCalculateEffect(EncounterActor actor, float skillTestResult) { }
  }
}