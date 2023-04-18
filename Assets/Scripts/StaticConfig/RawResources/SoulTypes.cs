using System.Collections.Generic;
using Common;
using UnityEngine;

namespace StaticConfig.RawResources {
  [CreateAssetMenu(menuName = "Config/SoulTypes")]
  public class SoulTypes : ScriptableObjectSingleton<SoulTypes> {
    protected override SoulTypes Self() => this;

    public RawResource violent;
    public RawResource treacherous;
    public RawResource diligent;
    public RawResource kind;

    public RawResource RandomType() {
      return Random.Range(0, 4) switch {
          0 => violent,
          1 => treacherous,
          2 => diligent,
          _ => kind,
      };
    }

    public List<RawResource> All() {
      return new List<RawResource> {
          violent,
          treacherous,
          diligent,
          kind,
      };
    }
  }
}