using System.Collections.Generic;
using System.Linq;
using State;
using Units;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

public class PrototypeSetup : MonoBehaviour {
  [SerializeField] private TileBase waterTile;
  [SerializeField] private TileBase raftTile;
  [SerializeField] private TileBase landTile;
  [SerializeField] private GameObject unitPrefab;
  
  private Grid _grid;
  private Tilemap _tilemap;
  private HashSet<Vector3Int> raftTiles = new();
  private HashSet<Vector3Int> landTiles = new();

  private void Awake() {
    _grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
    _tilemap = _grid.GetComponentInChildren<Tilemap>();

    IdentifyImportantTiles();
    AllocateUnitsForEncounter();
  }
  
  private void IdentifyImportantTiles() {
    for (int x = -100; x <= 100; x++) {
      for (int y = -100; y < 100; y++) {
        for (int z = 2; z >= 0; z--) {
          var tile = _tilemap.GetTile(new Vector3Int(x, y, z));
          if (tile == raftTile) {
            raftTiles.Add(new Vector3Int(x, y, z));
            // Debug.Log($"Found raft tile at {x},{y},{z}");
            break;
          }
          if (tile == landTile) {
            landTiles.Add(new Vector3Int(x, y, z));
            // Debug.Log($"Found land tile at {x},{y},{z}");
            break;
          }
        }
      }
    }
  }
  
  
  private void AllocateUnitsForEncounter() {
    var raftTileList = raftTiles.ToList();
    var landTileList = landTiles.ToList();
    HashSet<int> playerUnitCoords = new();
    HashSet<int> enemyUnitCoords = new();

    while (playerUnitCoords.Count < 3) {
      playerUnitCoords.Add(Random.Range(0, raftTileList.Count));
    }
    
    while (enemyUnitCoords.Count < 3) {
      enemyUnitCoords.Add(Random.Range(0, landTileList.Count));
    }

    foreach (int i in playerUnitCoords) {
      var loc = raftTileList[i];
      var unit = Instantiate(unitPrefab).GetComponent<UnitController>();
      unit.Init(new UnitState() {
          ControlSource = UnitControlSource.Player,
          CurrentHp = 20,
          MaxHp = 20,
          Faction = UnitFaction.PlayerParty,
          Position = loc,
      });
    }
    
    foreach (int i in enemyUnitCoords) {
      var loc = landTileList[i];
      var unit = Instantiate(unitPrefab).GetComponent<UnitController>();
      unit.Init(new UnitState() {
          ControlSource = UnitControlSource.AI,
          CurrentHp = 20,
          MaxHp = 20,
          Faction = UnitFaction.Enemy,
          Position = loc,
      });
    }
  }
}