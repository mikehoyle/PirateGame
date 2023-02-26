using RuntimeVars;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Encounter {
  public class RoundIndicator : MonoBehaviour {
    private const string RoundIndicatorText = "Round: ";
    [SerializeField] private IntegerVar currentRound;
    
    private Text _roundIndicator;
    
    private void Awake() {
      _roundIndicator = GetComponent<Text>();
    }

    private void Update() {
      _roundIndicator.text = RoundIndicatorText + currentRound.Value;
    }
  }
}