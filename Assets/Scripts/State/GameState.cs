using System;
using State.World;
using UnityEngine;

namespace State {
  /// <summary>
  /// A singleton which holds game state, intended to be serialized as a save-game
  /// </summary>
  [CreateAssetMenu(menuName = "State/GameState")]
  public class GameState : ScriptableObject {
    [NonSerialized] private static GameState _self;

    public WorldState world;
    public PlayerState player;

    public static GameState State {
      get {
        // TODO(P1): Convert to loading from file 
        return _self ??= Load();
      }
    }

    private static GameState Load() {
      if (Debug.isDebugBuild) {
        return Resources.Load<GameState>("Debug/DebugGameState");
      }
      
      // TODO(P1): This does nothing, should load from file
      // or default gamestate.
      return CreateInstance<GameState>();
    }
  }
}