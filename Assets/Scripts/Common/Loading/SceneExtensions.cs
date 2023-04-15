using System;

namespace Common.Loading {
  /// <summary>
  /// Holds all scene names for easy loading via SceneManager.Load().
  /// </summary>
  public static class SceneExtensions {
    // TODO(P3): Just use enum and load scene by build index
    public static string SceneName(this SceneId name) {
      return name switch {
          SceneId.Overworld => "OverworldScene",
          SceneId.Title => "TitleScene",
          SceneId.ShipBuilder => "ShipBuilderScene",
          SceneId.Encounter => "EncounterScene",
          _ => throw new NotImplementedException($"Attempted to load non-existent scene {(int)name}"),
      };
    }
  }
}