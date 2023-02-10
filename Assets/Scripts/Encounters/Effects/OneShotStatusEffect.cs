using UnityEngine;

namespace Encounters.Effects {
  /// <summary>
  /// Applies an effect a single time, after an optional animation delay
  /// </summary>
  [CreateAssetMenu(menuName = "Effects/OneShot")]
  public class OneShotStatusEffect : StatusEffect {
    public float delayInSeconds;

    private float _startTime;

    public override void Update() {
      _startTime += Time.deltaTime;
      if (_startTime >= delayInSeconds) {
        ApplyEffect();
        Destroy(this);
      }
    }
    
    public override void Init(EncounterActor victim) {
      base.Init(victim);
      _startTime = 0;
    }
  }
}