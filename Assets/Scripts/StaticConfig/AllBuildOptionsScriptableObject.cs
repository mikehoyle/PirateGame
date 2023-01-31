using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaticConfig {
  /// <summary>
  /// Represents all build options available in the game.
  /// </summary>
  [CreateAssetMenu(fileName = "AllBuildOptions", menuName = "ScriptableObjects/AllBuildOptions", order = 0)]
  public class AllBuildOptionsScriptableObject : ScriptableObject {
    public ConstructableScriptableObject[] buildOptions;

    [NonSerialized] private Dictionary<string, ConstructableScriptableObject> _buildMap;

    /// <summary>
    /// Because ScriptableObjects are very weird it's hard to reliably initialize them.
    /// To get around this, we just initialize the map the first time it's accessed if it
    /// isn't already
    /// </summary>
    public Dictionary<string, ConstructableScriptableObject> BuildMap {
      get {
        if (_buildMap == null) {
          _buildMap = new Dictionary<string, ConstructableScriptableObject>();
          foreach (var buildOption in buildOptions) {
            _buildMap.Add(buildOption.id, buildOption);
          }
        }
        return _buildMap;
      }
    }
  }
}