using System;
using Encounters;
using RuntimeVars.Encounters.Events;
using StaticConfig.Units;
using UnityEngine;
using UnityEngine.UI;

namespace Units {
  public class HpBarController : MonoBehaviour {
    [SerializeField] private ExhaustibleResource hpResource;
    [SerializeField] private EncounterEvents encounterEvents;
    
    private Slider _hpBar;
    private ExhaustibleResourceTracker _hpTracker;

    private void Awake() {
      _hpBar = GetComponent<Slider>();
      encounterEvents.encounterStart.RegisterListener(OnEncounterStart);
      encounterEvents.encounterEnd.RegisterListener(OnEncounterEnd);
      encounterEvents.unitAddedMidEncounter.RegisterListener(OnUnitAdded);
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      encounterEvents.encounterStart.UnregisterListener(OnEncounterStart);
      encounterEvents.encounterEnd.UnregisterListener(OnEncounterEnd);
      encounterEvents.unitAddedMidEncounter.UnregisterListener(OnUnitAdded);
    }

    private void OnEncounterStart() {
      // Only display during encounters.
      gameObject.SetActive(true);
    }

    private void OnUnitAdded(EncounterActor _) {
      // If this event is ever fired, we're mid-encounter, everyone should be showing HP bars
      // (but most importantly, the new units should).
      gameObject.SetActive(true);
    }

    private void OnEncounterEnd() {
      gameObject.SetActive(false);
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