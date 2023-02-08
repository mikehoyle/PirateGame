using System;
using System.Linq;
using Pathfinding;
using State;
using State.Unit;
using State.World;
using UnityEngine;
using Random = System.Random;

namespace Encounters {
  /// <summary>
  /// Encapsulates generation of an encounter. This happens at interaction time rather than
  /// upfront at map generation time, for a few reasons:
  ///  - It's not needed or seen before interaction.
  ///  - It reduces save file size and initial game load time.
  ///  - We can make dynamic choices mid-save about what is encountered.
  /// </summary>
  public class EncounterGenerator : MonoBehaviour {
    private Random _rng;
    
    private void Awake() {
      _rng = new Random();
    }

    /// <summary>
    /// Generate the tile for the first time. This is bound to evolve a LOT over the course of
    /// development, but for now, it's very simple.
    /// </summary>
    public void Generate(EncounterTile encounterTile) {
      GenerateTerrain(encounterTile);
      //GenerateUnits(encounterTile);
      encounterTile.isInitialized = true;
      // TODO
    }
    
    /// <summary>
    /// It's a simple flat rectangle of land... with maybe a little pool in the middle.
    /// </summary>
    private void GenerateTerrain(EncounterTile encounterTile) {
      var width = _rng.Next(6, 12);
      var height = _rng.Next(6, 12);
      var poolWidth = _rng.Next(2, 4);
      var poolHeight = _rng.Next(2, 4);
      var poolX = _rng.Next(0, 8);
      var poolY = _rng.Next(0, 8);

      for (int x = 0; x < width; x++) {
        for (int y = 0; y < height; y++) {
          encounterTile.terrain.Add(new Vector3Int(x, y, 0), TerrainType.Land);
        }
      }

      for (int x = 0; x < poolWidth; x++) {
        for (int y = 0; y < poolHeight; y++) {
          encounterTile.terrain.Remove(new Vector3Int(poolX + x, poolY + y));
        }
      }
    }
    
    /*/// <summary>
    /// This method is definitely un-fun but for now, just generate a number of units similar
    /// to what the player has, and hard-code their stats.
    /// </summary>
    private void GenerateUnits(EncounterTile encounterTile) {
      var playerUnits = GameState.State.player.roster.Count;
      var numUnits = _rng.Next(
          Math.Max(playerUnits - 1, 1), playerUnits + 2);

      var terrainPositions = encounterTile.terrain.Keys.OrderBy(_ => _rng.Next()).ToList();
      
      for (int i = 0; i < numUnits; i++) {
        var hp = _rng.Next(5, 10);
        var unit = ScriptableObject.CreateInstance<UnitState>();
        unit.startingPosition = terrainPositions[i];
        unit.maxHp = hp;
        unit.faction = UnitFaction.Enemy;
        unit.movementRange = _rng.Next(3, 5);
        unit.encounterState = ScriptableObject.CreateInstance<UnitEncounterState>();

        encounterTile.units.Add(unit);
      }
    }*/
  }
}