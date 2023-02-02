using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Common {
  /// <summary>
  /// Holds all scene names for easy loading via SceneManager.Load().
  /// </summary>
  public static class Scenes {
    public enum Name {
      Overworld,
      Tavern,
      ShipBuilder,
      Encounter,
    }

    // TODO(P3): Just use enum and load scene by build index
    public static string SceneName(this Name name) {
      return name switch {
          Name.Overworld => "OverworldScene",
          Name.Tavern => "TavernScene",
          Name.ShipBuilder => "ShipBuilderScene",
          Name.Encounter => "EncounterScene",
          _ => throw new NotImplementedException($"Attempted to load non-existent scene {(int)name}"),
      };
    }
    
    /// <summary>
    /// WARNING: This must be used in a MonoBehaviour with StartCoroutine.
    /// </summary>
    public static IEnumerator LoadAsync(Name sceneName) {
      var asyncLoad = SceneManager.LoadSceneAsync(sceneName.SceneName());

      // Wait until the asynchronous scene fully loads
      while (!asyncLoad.isDone) {
        yield return null;
      }
    }
  }
}