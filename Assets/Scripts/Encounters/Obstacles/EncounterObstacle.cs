using Common.Grid;
using HUD.Encounter.HoverDetails;
using StaticConfig.Encounters;
using UnityEngine;

namespace Encounters.Obstacles {
  /// <summary>
  /// Represents an obstacle placed on the grid.
  /// </summary>
  public class EncounterObstacle : MonoBehaviour, IPlacedOnGrid, IDisplayDetailsProvider {
    private SpriteRenderer _spriteRenderer;
    
    public ObstacleConfig Metadata { get; private set; }
    public Vector3Int Position { get; private set; }
    public bool BlocksAllMovement => true;
    public bool ClaimsTile => true;

    private void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ObstacleConfig obstacle, Vector3Int position) {
      Metadata = obstacle;
      Position = position;
      _spriteRenderer.sprite = obstacle.sprite;
      transform.position = GridUtils.CellBaseWorldStatic(position);
    }
    public DisplayDetails GetDisplayDetails() {
      return new DisplayDetails {
          // TODO(P3): Give obstacles flavorful names.
          Name = "Obstacle",
          AdditionalDetails = new() {
              $"HP: {Metadata.currentHp}/{Metadata.maxHp}",
          },
      };
    }
  }
}