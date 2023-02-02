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

    private void Awake() {
      _grid = IsometricGrid.Get();
    }

    public void SetUpMap(EncounterTile encounter) {
      foreach (var tile in encounter.Terrain) {
        // For now, ignoring tile type because there's only one. In the future, probably use a scriptable
        // object to define tile for different types.
        _grid.Tilemap.SetTile(tile.Key, landTile);
      }

      foreach (var unit in encounter.Units) {
        var unitController = Instantiate(unitPrefab).GetComponent<UnitController>();
        unitController.Init(unit);
      }
      
      // TODO(P0): pick up here once ship placement information is available.
    }
  }
}