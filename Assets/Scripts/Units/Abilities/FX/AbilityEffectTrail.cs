using System;
using System.Collections;
using Common.Grid;
using UnityEngine;

namespace Units.Abilities.FX {
  public class AbilityEffectTrail : MonoBehaviour {
    private void Awake() {
      // Ensure the trail (and this object) auto-destructs.
      GetComponent<TrailRenderer>().autodestruct = true;
    }

    public IEnumerator ExecuteThenDie(
        Vector3Int source,
        float sourceHeight,
        Vector3Int target,
        float targetHeight,
        float arcHeight,
        float speedTilesPerSecond) {
      var startPosition = GridUtils.CellCenterWorld(source);
      startPosition.y += sourceHeight;
      var targetPosition = GridUtils.CellCenterWorld(target);
      targetPosition.y += targetHeight;

      // Take into account diagonal straight-line distance for this visual representation
      var distance = Vector3.Distance(source, target);
      var durationSecs = distance / speedTilesPerSecond;
      var elapsedTime = 0f;
      
      // At very close range, any height can be silly, so dampen it down.
      arcHeight = Math.Min(arcHeight, arcHeight * (distance / 3)); 

      while (elapsedTime < durationSecs) {
        var t = elapsedTime / durationSecs;
        var position = Vector3.Lerp(startPosition, targetPosition, t);
        position.y += GetArcHeightAtTime(arcHeight, t);
        transform.position = position;
        yield return null;
        elapsedTime += Time.deltaTime;
      }
      
      // Trail renderer is auto-destruct so it will die shortly after moving ends.
    }

    /// <summary>
    /// Where t is a float 0-1.
    ///
    /// Defined using the quadratic formula, knowing that an arc of height n between 0 and 1 is:
    /// -4nx^2 + 4nx
    /// </summary>
    private float GetArcHeightAtTime(float arcHeight, float t) {
      return (-4 * arcHeight * (t * t)) + (4 * arcHeight * t);
    } 
  }
}