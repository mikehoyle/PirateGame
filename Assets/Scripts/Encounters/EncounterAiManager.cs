using System;
using Common.Events;
using UnityEngine;

namespace Encounters {
  public class EncounterAiManager : MonoBehaviour {
    [SerializeField] private EmptyGameEvent playerTurnEndEvent;
    [SerializeField] private EmptyGameEvent enemyTurnEndEvent;

    private void OnEnable() {
      playerTurnEndEvent.RegisterListener(OnPlayerTurnEnd);
    }

    private void OnDisable() {
      playerTurnEndEvent.UnregisterListener(OnPlayerTurnEnd);
    }

    private void OnPlayerTurnEnd() {
      // TODO(P0): Implement enemy AI, this currently ends enemy turn immediately
      enemyTurnEndEvent.Raise();
    }
  }
}