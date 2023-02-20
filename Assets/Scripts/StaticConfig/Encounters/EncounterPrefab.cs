using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace StaticConfig.Encounters {
  /// <summary>
  /// Defines a prefab shape. Terrain string is parsed as integers of height,
  /// starting at 0, where origin is the bottom-left of the provided string.
  /// </summary>
  [CreateAssetMenu(menuName = "Encounters/ObstaclePrefab")]
  public class EncounterPrefab : ScriptableObject {
    public bool flexibleHeight;
    [Multiline] public string terrainProfile;
    public SparseMatrix3d<ObstacleConfig> obstacles;
    
    private List<Vector3Int> _terrain;

    private void Awake() {
      ParseTerrainProfile();
    }

    private void OnValidate() {
      ParseTerrainProfile();
    }
    private void ParseTerrainProfile() {
      var lines = terrainProfile.Split("\n" , StringSplitOptions.RemoveEmptyEntries);
      
      _terrain = new();
      for (int y = lines.Length -1; y >= 0; y--) {
        for (int x = 0; x < lines[y].Length; x++) {
          var height = Convert.ToInt32(lines[y][x]);
          _terrain.Add(new Vector3Int(x, lines.Length - 1 - y, height));
        }
      }
    } 
  }
}