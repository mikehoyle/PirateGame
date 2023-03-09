using Common;
using Common.Loading;
using Controls;
using Encounters;
using RuntimeVars.Encounters.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class EncounterEndDisplay : MonoBehaviour, GameControls.IPressAnyKeyActions {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private string victoryText;
    [SerializeField] private string defeatText;
    [SerializeField] private Color victoryColor;
    [SerializeField] private Color defeatColor;
    
    private Text _outcomeText;
    private GameControls _controls;

    private void Awake() {
      _outcomeText = GetComponentInChildren<Text>();
      encounterEvents.encounterEnd.RegisterListener(OnEncounterEnd);
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      encounterEvents.encounterEnd.UnregisterListener(OnEncounterEnd);
    }

    private void OnEncounterEnd(EncounterOutcome outcome) {
      SetContent(outcome == EncounterOutcome.PlayerVictory);
      gameObject.SetActive(true);
    }

    private void OnEnable() {
      if (_controls == null) {
        _controls = new GameControls();
        _controls.PressAnyKey.SetCallbacks(this);
      }
      
      _controls.PressAnyKey.Enable();
    }

    private void OnDisable() {
      _controls?.PressAnyKey.Disable();
    }

    private void SetContent(bool isVictory) {
      if (isVictory) {
        _outcomeText.text = victoryText;
        _outcomeText.color = victoryColor;
      } else {
        _outcomeText.text = defeatText;
        _outcomeText.color = defeatColor;
      }
    }
    
    public void OnAnyKey(InputAction.CallbackContext context) {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}