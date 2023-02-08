using Construction;
using Pathfinding;
using State;
using State.World;
using Units;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Encounters {
  /// <summary>
  /// Sets up the scene-tree objects associated with the encounter
  /// </summary>
  public class EncounterSetup : MonoBehaviour {
    [SerializeField] private TileBase landTile;
    [SerializeField] private GameObject unitPrefab;
    
    private IsometricGrid _grid;
    private ShipSetup _shipSetup;
    private EncounterTerrain _terrain;

    private void Awake() {
      _grid = IsometricGrid.Get();
      _terrain = EncounterTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
    }

    public void SetUpMap(EncounterTile encounter, Vector3Int shipOffset) {
      foreach (var tile in encounter.terrain) {
        // For now, ignoring tile type because there's only one. In the future, probably use a scriptable
        // object to define tile for different types.
        _grid.Tilemap.SetTile(tile.Key, landTile);
        _terrain.MarkCellTraversable(tile.Key);
      }

      /*foreach (var unit in encounter.units) {
        var unitController = Instantiate(unitPrefab).GetComponent<UnitController>();
        unitController.Init(unit);
      }*/
      
      _shipSetup.SetupShip(shipOffset, includeUnits: true);
    }
  }
}