using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Animation;
using State.Unit;
using UnityEngine;

namespace Units.Abilities.AOE {
  /// <summary>
  /// Represents an Area-of-effect around a center (target) point.
  /// </summary>
  [Serializable]
  public class AreaOfEffect {
    /// <summary>
    /// Coordinates relative to the targeted point.
    /// </summary>
    [SerializeField] private List<Vector3Int> affectedCoords;
    [SerializeField] private Vector3Int targetPoint;

    public AreaOfEffect(List<Vector3Int> affectedPoints) {
      affectedCoords = affectedPoints;
      targetPoint = Vector3Int.zero;
    }
    
    public AreaOfEffect(List<Vector3Int> affectedPoints, Vector3Int targetPoint) {
      affectedCoords = affectedPoints;
      this.targetPoint = targetPoint;
    }

    public AreaOfEffect WithTarget(Vector3Int target) {
      return new AreaOfEffect(affectedCoords, target);
    }

    /// <summary>
    /// Assumes the provided AOE pattern is facing +Y (NW)
    /// </summary>
    public AreaOfEffect WithTargetAndRotation(Vector3Int source, Vector3Int target) {
      var direction = FacingUtilities.DirectionBetween((Vector2Int)source, (Vector2Int)target);
      return WithRotation(direction).WithTarget(target);
    }

    public AreaOfEffect WithRotation(FacingDirection direction) {
      var newAffectedPoints = new List<Vector3Int>();
      foreach (var affectedCoord in affectedCoords) {
        newAffectedPoints.Add(direction.RotateAroundOrigin(affectedCoord));
      }
      return new AreaOfEffect(newAffectedPoints);
    }

    public IEnumerable<Vector3Int> AffectedPoints() {
      return affectedCoords.Select(coord => coord + targetPoint);
    }

    public bool AffectsPoint(Vector3Int point) {
      return AffectedPoints().Any(affectedPoint => affectedPoint == point);
    }

    public Vector3Int GetTarget() {
      return targetPoint;
    }
  }
}