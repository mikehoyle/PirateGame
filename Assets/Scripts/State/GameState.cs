using System;
using UnityEditor.Hardware;

namespace State {
  /// <summary>
  /// A singleton which holds game state, intended to be serialized as a save-game
  /// </summary>
  [Serializable]
  public class GameState {
    [NonSerialized] private static GameState _self;

    private GameState() {
      World = new WorldState();
      Player = new PlayerState();
    }

    public WorldState World;
    public PlayerState Player;

    public static GameState State {
      get {
        // TODO(P1): Convert to loading from file
        return _self ??= new GameState();
      }
    }

    public static void Load() {
      // TODO(P1): Convert to loading from file
      // Currently effective no-op
      _self ??= new GameState();
    }
  }
}