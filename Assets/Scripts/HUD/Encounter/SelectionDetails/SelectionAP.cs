using RuntimeVars.Encounters;
using StaticConfig.Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter.SelectionDetails {
  public class SelectionAP : MonoBehaviour{
    [SerializeField] private CurrentSelection currentSelection;
    [SerializeField] private ExhaustibleResource apResource;
    
    private Text _text;

    private void Awake() {
      _text = GetComponent<Text>();
    }

    private void Update() {
      currentSelection.selectedUnit.MatchSome(
          unit => _text.text = $"AP: {unit.EncounterState.GetResourceAmount(apResource)}");
    }
  }
}