using Common;
using Controls;
using Events;
using Terrain;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounters.Managers {
  public class MouseHoverTracker : MonoBehaviour, GameControls.IHoverTileActions {
    private UiInteractionTracker _uiInteraction;
    private SceneTerrain _terrain;
    private Vector3Int _lastKnownHoveredTile = new(int.MaxValue, int.MaxValue, 0);
    private GameControls _controls;

    private void Awake() {
      _uiInteraction = GetComponent<UiInteractionTracker>();
      _terrain = SceneTerrain.Get();
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.HoverTile.SetCallbacks(this);
      }
      _controls.HoverTile.Enable();
    }

    private void OnDisable() {
      _controls?.HoverTile.Disable();
    }

    public void OnPoint(InputAction.CallbackContext context) {
      if (!context.performed || _uiInteraction.isPlayerHoveringUi) {
        // Ignore UI-intended events
        return;
      }
      
      var hoveredTile = _terrain.TileAtScreenCoordinate(context.ReadValue<Vector2>());
      if (hoveredTile == _lastKnownHoveredTile) {
        // No need to update if the selected tile is the same
        return;
      }
      _lastKnownHoveredTile = hoveredTile;
      Dispatch.Encounters.MouseHover.Raise(hoveredTile);
    }
  }
}