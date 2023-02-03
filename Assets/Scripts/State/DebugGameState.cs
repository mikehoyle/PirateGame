using System.Collections.Generic;
using System.Linq;
using Overworld.MapGeneration;
using StaticConfig;
using UnityEngine;

namespace State {
  /// <summary>
  /// Creates a static game state object for debugging purposes, rather than randomly
  /// generating or loading from disk.
  /// TODO(P1): Actually use proper generation and load/save at appropriate times, not this
  /// </summary>
  public static class DebugGameState {
    private const int PlayerRosterSize = 3;
    private const int ShipWidth = 3;
    private const int ShipHeight = 3;
    
    public static GameState Generate(GameState gameState) {
      GeneratePlayerState(gameState);
      GenerateOverworldState(gameState);
      return gameState;
    }
    
    private static void GeneratePlayerState(GameState gameState) {
      GenerateShipState(gameState.Player);
      GenerateInventory(gameState.Player);
      GenerateRoster(gameState.Player);
    }

    private static void GenerateOverworldState(GameState gameState) {
      gameState.World = new OverworldGenerator(width: 100, height: 100, seed: 1).GenerateWorld();
    }

    private static void GenerateShipState(PlayerState playerState) {
      for (int x = 0; x < ShipWidth; x++) {
        for (int y = 0; y < ShipHeight; y++) {
          // Very error-prone, don't do this in prod code.
          playerState.Ship.Components.Add(new Vector3Int(x, y, 0), "foundation");
        }
      }
    }

    private static void GenerateInventory(PlayerState playerState) {
      playerState.Inventory.DebugOnlySetQuantity("lumber", 45);
    }

    private static void GenerateRoster(PlayerState playerState) {
      var shipFoundations = playerState.Ship.Components.Keys.ToList();
      for (int i = 0; i < PlayerRosterSize; i++) {
        playerState.Roster.Add(new UnitState() {
            ControlSource = UnitControlSource.Player,
            MaxHp = 20,
            Faction = UnitFaction.PlayerParty,
            // Just place on first known spot, obvious cause for future bugs, but this
            // is only for early debugging
            StartingPosition = shipFoundations[i],
            MovementRange = 3,
        });
      }
    }
  }
}