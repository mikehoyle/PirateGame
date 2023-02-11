using UnityEngine;

namespace Encounters.Effects {
  /// <summary>
  /// Applies an effect a single time, after an optional animation delay
  /// </summary>
  [CreateAssetMenu(menuName = "Effects/OneShot")]
  public class OneShotStatusEffect : StatusEffect {
    public float delayInSeconds;

    private float _startTime;
    
    public override bool UpdateAndMaybeDestroy(EncounterActor actor) {
      _startTime += Time.deltaTime;
      if (_startTime >= delayInSeconds) {
        EnactEffect(actor);
        Destroy(this);
        return true;
      }
      return false;
    }
    
    public override void OnApply() {
      _startTime = 0;
    }
  }
}