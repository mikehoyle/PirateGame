using UnityEngine;
using UnityEngine.UI;

namespace Units {
  public class HpBarController : MonoBehaviour {
    private Slider _hpBar;
    private UnitEncounterManager _unit;

    private void Awake() {
      _hpBar = GetComponent<Slider>();
      enabled = false;
    }

    private void Update() {
      _hpBar.value = _unit.CurrentHp;
    }

    public void Init(UnitEncounterManager unit, int maxHp) {
      _unit = unit;
      _hpBar.maxValue = maxHp;
      _hpBar.minValue = 0;
      _hpBar.value = maxHp;
      enabled = true;
    }
  }
}