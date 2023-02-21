using System;
using Random = UnityEngine.Random;

namespace StaticConfig.Encounters {
  /// <summary>
  /// This class necessary because SerializableDictionary needs it for the inspector view.
  /// </summary>
  [Serializable]
  public class ObstacleVariants {
    public ObstacleConfig[] variants;

    public ObstacleConfig RandomVariant() {
      return variants[Random.Range(0, variants.Length)];
    }
  }
}