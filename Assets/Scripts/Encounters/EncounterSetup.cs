using Construction;
using Encounters.Enemies;
using State.World;
using Terrain;
using UnityEngine;

namespace Encounters {
  /// <summary>
  /// Sets up the scene-tree objects associated with the encounter.
  /// </summary>
  public class EncounterSetup : MonoBehaviour {
    [SerializeField] private Sprite landSprite;
    [SerializeField] private GameObject enemyPrefab;
    
    private ShipSetup _shipSetup;
    private SceneTerrain _terrain;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _shipSetup = GetComponent<ShipSetup>();
    }

    public void SetUpMap(EncounterTile encounter, Vector3Int shipOffset) {
      foreach (var tile in encounter.terrain) {
        // For now, ignoring tile type because there's only one. In the future, probably use a scriptable
        // object to define tile for different types.
        _terrain.AddTerrain(tile.Key, landSprite);
      }

      foreach (var enemy in encounter.enemies) {
        var unitController = Instantiate(enemyPrefab).GetComponent<EnemyUnitController>();
        unitController.Init(enemy);
      }
      
      _shipSetup.SetupShip(shipOffset, includeUnits: true);
    }
  }
}