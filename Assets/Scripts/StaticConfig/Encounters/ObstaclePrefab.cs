using Common;
using Units.Abilities.AOE;
using UnityEngine;

namespace StaticConfig.Encounters {
  /// <summary>
  /// Defines a prefab shape. Terrain string defines an AOE as per <code>AreaOfEffect</code>,
  /// where "affected" tiles must remain clear, for the prefab.
  ///
  /// The provided obstacle matrix coordinates are relative to (0,0), the target of the provided AOE.
  ///
  /// If multiple obstacles are provided for a given coordinate, then the various options are assumed to be
  /// variants, which will be randomly chosen at encounter generation time.
  /// </summary>
  [CreateAssetMenu(menuName = "Encounters/ObstaclePrefab")]
  public class ObstaclePrefab : ScriptableObject {
    [Multiline] public string requiredFootprint;
    public SparseMatrix3d<ObstacleVariants> obstacles;
    
    public AreaOfEffect Footprint { get; private set; }

    private void Awake() {
      ParseFootprint();
    }

    private void OnValidate() {
      ParseFootprint();
    }

    private void ParseFootprint() {
      if (string.IsNullOrEmpty(requiredFootprint)) {
        return;
      }
      Footprint = AoeParser.ParseAreaOfEffectRaw(requiredFootprint);
    }
  }
}