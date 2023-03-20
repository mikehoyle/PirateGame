using Encounters;
using Events;
using FMODUnity;
using UnityEngine;

namespace HUD.Audio {
  public class UiAudioListener : MonoBehaviour {
    [SerializeField] private EventReference tileHoverSound;
    [SerializeField] private EventReference unitSelectedSound;

    private void OnEnable() {
      Dispatch.Encounters.MouseHover.RegisterListener(OnTileHover);
      Dispatch.Encounters.UnitSelected.RegisterListener(OnUnitSelected);
    }

    private void OnDisable() {
      Dispatch.Encounters.MouseHover.UnregisterListener(OnTileHover);
      Dispatch.Encounters.UnitSelected.UnregisterListener(OnUnitSelected);
    }

    private void OnTileHover(Vector3Int _) {
      RuntimeManager.PlayOneShot(tileHoverSound);
    }

    private void OnUnitSelected(EncounterActor _) {
      RuntimeManager.PlayOneShot(unitSelectedSound);
    }
  }
}