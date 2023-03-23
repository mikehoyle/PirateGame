using System.Collections.Generic;
using Units.Abilities.AOE;
using UnityEngine;

namespace StaticConfig.Encounters {
  [CreateAssetMenu(menuName = "Encounters/TerrainPrefabs")]
  public class TerrainPrefabs : ScriptableObject {
    [SerializeField] private List<TextAsset> terrainConfigs;

    public int Count => terrainConfigs.Count;
    
    private TerrainPrefabs() {
      terrainConfigs = new();
    }

    public AreaOfEffect GetTerrainMap(int index) {
      if (index < 0 || index >= terrainConfigs.Count) {
        return null;
      }
      var config = terrainConfigs[index].text;
      if (string.IsNullOrEmpty(config)) {
        return null;
      }
      return AoeParser.ParseAreaOfEffect(config);
    }
  }
}