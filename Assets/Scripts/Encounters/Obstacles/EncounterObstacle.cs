using StaticConfig.Encounters;
using Terrain;
using UnityEngine;

namespace Encounters.Obstacles {
  /// <summary>
  /// Represents an obstacle placed on the grid.
  /// </summary>
  public class EncounterObstacle : MonoBehaviour, IPlacedOnGrid {
    private SpriteRenderer _spriteRenderer;
    
    public ObstacleConfig Metadata { get; private set; }
    public Vector3Int Position { get; private set; }

    private void Awake() {
      _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(ObstacleConfig obstacle, Vector3Int position) {
      Metadata = obstacle;
      Position = position;
      _spriteRenderer.sprite = obstacle.sprite;
      transform.position = SceneTerrain.CellBaseWorldStatic(position);
    }
  }
}