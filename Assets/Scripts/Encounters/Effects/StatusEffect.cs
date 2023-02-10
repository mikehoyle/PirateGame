using System;
using StaticConfig.Units;
using UnityEngine;

namespace Encounters.Effects {
  [CreateAssetMenu(menuName = "Encounters/Effects/StatusEffect")]
  public class StatusEffect : ScriptableObject {
    [Serializable]
    public class ExhaustibleResourceEffect {
      public ExhaustibleResource resource;
      public int diff;
    }

    public ExhaustibleResourceEffect[] exhaustibleResourceEffects;
    
    private EncounterActor _victim;
    
    public virtual void Update() {}

    public virtual void Init(EncounterActor victim) {
      _victim = victim;
    }

    protected void ApplyEffect() {
      Debug.Log($"Applying effect to to victim {_victim.name}");
      foreach (var exhaustibleResourceEffect in exhaustibleResourceEffects) {
        _victim.EncounterState.ExpendResource(
            exhaustibleResourceEffect.resource, -exhaustibleResourceEffect.diff);
      }
    }
  }
}