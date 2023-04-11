using System.Linq;
using Construction;
using Controls;
using Events;
using State;
using State.World;
using Terrain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters.ShipPlacement {
  /// <summary>
  /// Manages the flow where the player gets to choose where their ship is placed.
  /// </summary>
  public class ShipPlacementManager : MonoBehaviour, GameControls.IShipPlacementActions {
    [SerializeField] private GameObject ghostShipPrefab;
    
    private ShipSetup _shipSetup;
    private GameObject _ghostShip;
    private GameControls _controls;
    private SceneTerrain _terrain;
    private TerrainProfile _encounterProfile;
    private TerrainProfile _shipProfile;
    private Vector3Int? _currentShipOffset;

    private void Awake() {
      _shipSetup = GetComponent<ShipSetup>();
      _ghostShip = Instantiate(ghostShipPrefab);
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.ShipPlacement.SetCallbacks(this);
      }
      _controls.ShipPlacement.Enable();
      _shipSetup.SetupGhostShip(_ghostShip.transform);
      _ghostShip.SetActive(false);
    }

    private void OnDisable() {
      _controls.ShipPlacement.Disable();
    }

    public void BeginShipPlacement(EncounterWorldTile encounter) {
      _encounterProfile = TerrainProfile.BuildFrom(encounter.terrain.Keys);
      _shipProfile = TerrainProfile.BuildFrom(
          GameState.State.player.ship.foundations.Keys);
      enabled = true;
      
      // MAYBE TEMPORARY, but just place the ship for the player for now.
      var maxY = _terrain.AllTiles.Max(tile => tile.y);
      _shipSetup.SetupShip(new Vector3Int(4, maxY + 1), includeUnits: true);
      Dispatch.Encounters.ShipPlaced.Raise();
      enabled = false;
    }
    
    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed || !_currentShipOffset.HasValue) {
        return;
      }
      
      _shipSetup.SetupShip(_currentShipOffset.Value, includeUnits: true);
      Dispatch.Encounters.ShipPlaced.Raise();
      enabled = false;
    }
    
    public void OnPoint(InputAction.CallbackContext context) {
      if (context.canceled) {
        if (_ghostShip != null) {
          _ghostShip.SetActive(false);
        }
        return;
      }
      if (!context.performed) {
        return;
      }
      
      _ghostShip.SetActive(true);
      var hoveredTile = _terrain.TileAtScreenCoordinate(context.ReadValue<Vector2>());
      var overlap = _shipProfile.CalculateEdgeOverlap(hoveredTile, _encounterProfile);
      var offset = overlap.SuggestedSnapOffset();
      _currentShipOffset = hoveredTile + offset;

      if (_currentShipOffset.HasValue) {
        _ghostShip.transform.position = _terrain.CellBaseWorld(_currentShipOffset.Value);
      } else {
        // TODO(P2): This is bad UX but is easy for now.
        _ghostShip.SetActive(false);
      }
    }
  }
}