using HUD.Encounter;
using RuntimeVars;
using UnityEngine;
using UnityEngine.UI;

namespace Encounters {
  public class EncounterHUD : MonoBehaviour {
    private const string RoundIndicatorText = "Round: ";
    [SerializeField] private IntegerVar currentRound;
    
    private Text _roundIndicator;
    
    private void Awake() {
      _roundIndicator = transform.Find("RoundIndicator").GetComponent<Text>();
    }

    private void Update() {
      _roundIndicator.text = RoundIndicatorText + currentRound.Value;
    }
  }
}