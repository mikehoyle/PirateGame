using Units.Abilities;
using UnityEngine;

namespace Encounters.Effects {
  public abstract class AppliedStatusEffect : MonoBehaviour {
    public virtual void PreCalculateEffect(UnitAbility.AbilityExecutionContext context, float skillTestResult) { }
  }
}