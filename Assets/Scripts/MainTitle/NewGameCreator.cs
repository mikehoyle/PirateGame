using Overworld.MapGeneration;
using State;
using StaticConfig.RawResources;
using UnityEngine;

namespace MainTitle {
  /// <summary>
  /// For now, creates a game state for a fresh game session. Will require significant
  /// refinement in the future.
  /// </summary>
  public class NewGameCreator : MonoBehaviour {
    // This is a really hacky way to load this into memory
    [SerializeField] private SoulTypes soulTypes;
    [SerializeField] private PlayerState newGamePlayerState;
    [SerializeField] private int mapRadius = 30;

    public void SetUpNewGame() {
      var gameState = GameState.New();
      gameState.world = new WorldGenerator(mapRadius).GenerateWorld();
      gameState.player = newGamePlayerState.DeepCopy();
    }
  }
}