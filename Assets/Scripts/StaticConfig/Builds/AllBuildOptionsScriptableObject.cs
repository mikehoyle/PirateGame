using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaticConfig.Builds {
  /// <summary>
  /// Represents all build options available in the game.
  /// </summary>
  [CreateAssetMenu(fileName = "AllBuildOptions", menuName = "Config/AllBuildOptions", order = 0)]
  public class AllBuildOptionsScriptableObject : ScriptableObject {
    public ConstructableObject[] buildOptions;
  }
}