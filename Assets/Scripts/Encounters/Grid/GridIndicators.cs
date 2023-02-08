using UnityEngine;

namespace Encounters.Grid {
  public class GridIndicators : MonoBehaviour {
    public PathIndicator PathIndicator { get; private set; }
    public RangeIndicator RangeIndicator { get; private set; }
    public TargetingIndicator TargetingIndicator { get; private set; }
    
    private void Awake() {
      PathIndicator = GetComponentInChildren<PathIndicator>();
      RangeIndicator = GetComponentInChildren<RangeIndicator>();
      TargetingIndicator = GetComponentInChildren<TargetingIndicator>();
    }

    public void Clear() {
      PathIndicator.Clear();
      RangeIndicator.Clear();
      TargetingIndicator.Clear();
    }

    public static GridIndicators Get() {
      return GameObject.FindWithTag(Tags.GridIndicators).GetComponent<GridIndicators>();
    }
  }
}