using System;
using Common.Events;
using Construction;
using Controls;
using RuntimeVars.Encounters.Events;
using State;
using State.World;
using Terrain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters {
  /// <summary>
  /// Manages the flow where the player gets to choose where their ship is placed.
  /// </summary>
  public class ShipPlacementManager : MonoBehaviour, GameControls.IShipPlacementActions {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private GameObject ghostShipPrefab;
    
    private EncounterTile _encounter;
    private ShipSetup _shipSetup;
    private GameObject _ghostShip;
    private GameControls _controls;
    private SceneTerrain _terrain;

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
    }

    private void OnDisable() {
      _controls.ShipPlacement.Disable();
    }

    public void BeginShipPlacement(EncounterTile encounter) {
      _encounter = encounter;
      enabled = true;
    }

    // For now, just put the ship next to the terrain in the y+ direction.
    private Vector3Int GetShipPlacementOffset(ShipState ship) {
      var terrainBounds = _encounter.terrain.GetBoundingRect();
      var shipBoundingRect = ship.components.GetBoundingRect();
      return new Vector3Int(
          terrainBounds.xMin - shipBoundingRect.xMin,
          (terrainBounds.yMax + 1) - shipBoundingRect.yMin,
          0
      );
    }
    
    public void OnClick(InputAction.CallbackContext context) {
      if (!context.performed) {
        return;
      }
      
      // TODO(P0): Obviously address this
      _shipSetup.SetupShip(GetShipPlacementOffset(GameState.State.player.ship), includeUnits: true);
      encounterEvents.encounterReadyToStart.Raise();
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
      
      _ghostShip.transform.position = _terrain.CellBaseWorld(
          _terrain.TileAtScreenCoordinate(context.ReadValue<Vector2>()));;
    }
  }
}