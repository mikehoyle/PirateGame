﻿using System;
using Common;
using Encounters;
using RuntimeVars.Encounters.Events;
using StaticConfig.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
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
      if (SceneManager.GetActiveScene().name != Scenes.Name.Encounter.SceneName()) {
        // This is super janky, but the easiest way for now.
        gameObject.SetActive(false);
        return;
      }
      
      var unit = GetComponentInParent<EncounterActor>();
      if (!unit.EncounterState.TryGetResourceTracker(hpResource, out _hpTracker)) {
        gameObject.SetActive(false);
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