using Encounters;
using Events;
using FMODUnity;
using Terrain;
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

    private void OnTileHover(Vector3Int position) {
      // OPTIMIZE: if there is a problem with calling this so regularly
      var sceneTerrain = SceneTerrain.Get();
      if (sceneTerrain && sceneTerrain.GetTile((Vector2Int)position) != null) {
        RuntimeManager.PlayOneShot(tileHoverSound);
      }
    }

    private void OnUnitSelected(EncounterActor _) {
      RuntimeManager.PlayOneShot(unitSelectedSound);
    }
  }
}