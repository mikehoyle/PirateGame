using Encounters;
using StaticConfig.Units;
using UnityEngine;
using UnityEngine.UI;

namespace Units {
  public class HpBarController : MonoBehaviour {
    [SerializeField] private ExhaustibleResource hpResource;
    
    private Slider _hpBar;
    private ExhaustibleResourceTracker _hpTracker;

    private void Awake() {
      _hpBar = GetComponent<Slider>();
    }

    private void Start() {
      var unit = GetComponentInParent<EncounterActor>();

      if (!unit.EncounterState.TryGetResourceTracker(hpResource, out _hpTracker)) {
        enabled = false;
        return;
      }
      
      _hpBar.maxValue = _hpTracker.max;
      _hpBar.minValue = _hpTracker.min;
      _hpBar.value = _hpTracker.current;
    }

    private void Update() {
      _hpBar.value = _hpTracker.current;
    }
  }
}