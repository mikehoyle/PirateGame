using System;
using State;
using State.World;
using UnityEngine;

namespace Encounters {
  public class EncounterLoader : MonoBehaviour {
    [SerializeField] private GameObject encounterManagerPrefab;
    
    private EncounterTile _encounter;
    private EncounterGenerator _encounterGenerator;
    private EncounterSetup _encounterSetup;
    private IsometricGrid _grid;

    private void Awake() {
      _encounter = GameState.State.World.GetActiveTile().DownCast<EncounterTile>();
      if (_encounter == null) {
        throw new NotSupportedException(
            "Cannot load an encounter on a non-encounter tile." +
            $"Tile: {GameState.State.Player.OverworldGridPosition}," +
            $"Tile type: {GameState.State.World.GetActiveTile().TileType}");
      }

      _encounterGenerator = GetComponent<EncounterGenerator>();
      _encounterSetup = GetComponent<EncounterSetup>();
      _grid = IsometricGrid.Get();
    }

    private void Start() {
      if (!_encounter.IsInitialized) {
        _encounterGenerator.Generate(_encounter);
      }
      // TODO(P0): determine and place player ship position
      
      _encounterSetup.SetUpMap(_encounter);
      _grid.SetupPathfinding(_encounter);
      Instantiate(encounterManagerPrefab);
      Destroy(gameObject);
    }
  }
}