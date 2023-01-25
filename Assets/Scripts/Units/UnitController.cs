using State;
using UnityEngine;
using UnityEngine.UI;

namespace Units {
  public class UnitController : MonoBehaviour {
    private UnitState _state;
    private Slider _hpBar;
    private Grid _grid;

    private void Awake() {
      _grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      _hpBar = transform.Find("UnitIndicators").GetComponentInChildren<Slider>();
    }

    private void Update() {
      _hpBar.value = _state.CurrentHp;
      transform.position = _grid.GetCellCenterWorld(_state.Position);
    }

    public void Init(UnitState state) {
      _state = state;
      _hpBar.maxValue = _state.MaxHp;
      _hpBar.minValue = 0;
      _hpBar.value = _state.CurrentHp;

      transform.position = _grid.GetCellCenterWorld(_state.Position);
    }
  }
}