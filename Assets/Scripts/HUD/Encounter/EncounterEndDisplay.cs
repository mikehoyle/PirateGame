using System.Text;
using Common.Loading;
using Controls;
using Encounters;
using Events;
using RuntimeVars;
using State;
using State.World;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class EncounterEndDisplay : MonoBehaviour, GameControls.IPressAnyKeyActions {
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private string victoryText;
    [SerializeField] private string defeatText;
    [SerializeField] private Color victoryColor;
    [SerializeField] private Color defeatColor;
    
    private Text _outcomeText;
    private Text _bountyText;
    private GameControls _controls;

    private void Awake() {
      _outcomeText = transform.Find("OutcomeText").GetComponent<Text>();
      _bountyText = transform.Find("BountyText").GetComponent<Text>();
      Dispatch.Encounters.EncounterEnd.RegisterListener(OnEncounterEnd);
      gameObject.SetActive(false);
    }

    private void OnDestroy() {
      Dispatch.Encounters.EncounterEnd.UnregisterListener(OnEncounterEnd);
    }

    private void OnEncounterEnd(EncounterOutcome outcome) {
      Debug.Log($"Encounter ending with outcome: {outcome}");
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
      
      
      if (isVictory) {
        var encounter = GameState.State.world.GetActiveTile().DownCast<EncounterWorldTile>();
        var lootText = new StringBuilder();
        foreach (var playerUnit in playerUnitsInEncounter) {
          if (playerUnit != null) {
            foreach (var collectableInstance in playerUnit.CollectablesAcquired) {
              lootText.Append(collectableInstance.DisplayString());
            }
          }
        }

        // Ignore XP for now, it's on the chopping block
        /*foreach (var unit in GameState.State.player.roster) {
          unit.GrantXp(ExperienceCalculations.GetXpForVictoryInEncounter(encounter));
        }*/

        _bountyText.text = lootText.ToString();
        encounter.MarkDefeated();
      } else {
        _bountyText.gameObject.SetActive(false);
      }
    }
    
    public void OnAnyKey(InputAction.CallbackContext context) {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}