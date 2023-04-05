using System;
using Common;
using Encounters.Effects;
using StaticConfig.RawResources;
using UnityEngine;

namespace StaticConfig.Encounters {
  [CreateAssetMenu(menuName = "Config/SoulBuffs")]
  public class SoulBuffs : ScriptableObjectSingleton<SoulBuffs> {
    [Serializable]
    public class SoulBuffPair {
      public RawResource soulType;
      [SerializeReference] [SerializeReferenceButton]
      public StatusEffect effect;
    }

    public SoulBuffPair[] buffs;

    public StatusEffect GetBuff(RawResource soulType) {
      foreach (var buffPair in buffs) {
        if (buffPair.soulType == soulType) {
          return buffPair.effect;
        }
      }
      
      // Should be unreachable
      Debug.LogWarning($"Could not find soul buff for type {soulType.displayName}");
      return new OneShotStatusEffect();
    }
    
    protected override SoulBuffs Self() {
      return this;
    }
  }
}