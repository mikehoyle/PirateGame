using HUD.Encounter;
using UnityEngine;
using UnityEngine.UI;

namespace Encounters {
  public class EncounterHUD : MonoBehaviour {
    private const string RoundIndicatorText = "Round: ";
    [SerializeField] private GameObject encounterEndDisplayPrefab;
    
    private Text _roundIndicator;
    
    private void Awake() {
      _roundIndicator = transform.Find("RoundIndicator").GetComponent<Text>();
    }

    public void SetRound(int round) {
      _roundIndicator.text = RoundIndicatorText + round;
    }

    public void EndEncounter(bool isVictory) {
      Instantiate(encounterEndDisplayPrefab, transform).GetComponent<EncounterEndDisplay>().Init(isVictory);
    }
  }
}