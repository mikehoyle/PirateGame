using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace Encounters.Managers {
  public class SceneMusic : MonoBehaviour {
    [SerializeField] private EventReference battleMusic;
    
    private EventInstance _music;

    private void Start() {
      if (!battleMusic.IsNull) {
        _music = RuntimeManager.CreateInstance(battleMusic);
        _music.start();
      }
    }

    private void OnDestroy() {
      if (_music.isValid()) {
        _music.stop(STOP_MODE.ALLOWFADEOUT);
      }
    }
  }
}