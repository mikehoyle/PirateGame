using Encounters;
using FMODUnity;
using RuntimeVars.Encounters.Events;
using UnityEngine;

namespace HUD.Audio {
  public class UiAudioListener : MonoBehaviour {
    [SerializeField] private EncounterEvents encounterEvents;
    [SerializeField] private EventReference tileHoverSound;
    [SerializeField] private EventReference unitSelectedSound;

    private void OnEnable() {
      encounterEvents.mouseHover.RegisterListener(OnTileHover);
      encounterEvents.unitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDisable() {
      encounterEvents.mouseHover.UnregisterListener(OnTileHover);
      encounterEvents.unitSelected.UnregisterListener(OnUnitSelected);
    }

    private void OnTileHover(Vector3Int _) {
      RuntimeManager.PlayOneShot(tileHoverSound);
    }

    private void OnUnitSelected(EncounterActor _) {
      RuntimeManager.PlayOneShot(unitSelectedSound);
    }
  }
}