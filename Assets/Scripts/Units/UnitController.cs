using System.Collections.Generic;
using State;
using UnityEngine;
using UnityEngine.UI;

namespace Units {
  public class UnitController : MonoBehaviour {
    private Slider _hpBar;
    private Grid _grid;
    
    
    public UnitState State { get; private set; }
    // TODO(P1): This doesn't seem to be the true center.
    public Vector3 WorldPosition => _grid.GetCellCenterWorld(State.Position);

    private void Awake() {
      _grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      _hpBar = transform.Find("UnitIndicators").GetComponentInChildren<Slider>();
    }

    private void Update() {
      _hpBar.value = State.CurrentHp;
      transform.position = WorldPosition;
    }

    public void Init(UnitState state) {
      State = state;
      _hpBar.maxValue = State.MaxHp;
      _hpBar.minValue = 0;
      _hpBar.value = State.CurrentHp;

      transform.position = WorldPosition;
    }
    
    /// <returns>Whether the unit is eligible to move along the path</returns>
    public bool MoveAlongPath(LinkedList<Vector3Int> path) {
      var pathLength = path.Count;
      if (pathLength == 0 || path.Count - 1 > State.MovementRange) {
        return false;
      }
      
      // DO NOT SUBMIT only for testing
      State.Position = path.Last.Value;
      Debug.Log($"Unit current grid position: {State.Position}");
      Debug.Log($"Unit current world position: {WorldPosition}");
      return true;
    }
  }
}