using System;
using Encounters;
using Encounters.Grid;
using Terrain;
using UnityEngine;

namespace Units.Abilities.Range {
  [Serializable]
  public abstract class AbilityRange {
    [SerializeField] public bool requiresLineOfSight = true;

    public bool IsInRange(EncounterActor actor, Vector3Int source, Vector3Int target) {
      if (requiresLineOfSight && !HasLineOfSight(source, target)) {
        return false;
      }
      return IsInRangeInternal(actor, source, target);
    }

    protected abstract bool IsInRangeInternal(EncounterActor actor, Vector3Int source, Vector3Int target);
    public abstract void DisplayTargetingRange(EncounterActor actor, GridIndicators indicators, Vector3Int source);

    /// <summary>
    /// Uses Supercover algorithm as found on
    /// https://www.redblobgames.com/grids/line-drawing.html#supercover
    /// </summary>
    protected bool HasLineOfSight(Vector3Int source, Vector3Int target) {
      var dx = target.x - source.x;
      var dy = target.y - source.y;
      var nx = Math.Abs(dx);
      var ny = Math.Abs(dy);
      var sign_x = dx > 0 ? 1 : -1;
      var sign_y = dy > 0? 1 : -1;

      var tile = new Vector3Int(source.x, source.y);
      var ix = 0;
      var iy = 0;
      for (; ix < nx || iy < ny;) {
        var decision = (1 + 2*ix) * ny - (1 + 2*iy) * nx;
        if (decision == 0) {
          // next step is diagonal
          tile.x += sign_x;
          tile.y += sign_y;
          ix++;
          iy++;
        } else if (decision < 0) {
          // next step is horizontal
          tile.x += sign_x;
          ix++;
        } else {
          // next step is vertical
          tile.y += sign_y;
          iy++;
        }

        // Source cannot block itself, and we can target items that block line of sight,
        // just not past them.
        if (tile != source && tile != target) {
          if (SceneTerrain.DoesTileHaveObjectWhere(tile, item => item.BlocksLineOfSight)) {
            return false;
          }
        }
      }
      return true;
    }
  }
}