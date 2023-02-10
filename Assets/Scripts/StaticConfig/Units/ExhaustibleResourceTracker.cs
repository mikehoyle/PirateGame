using System;
using UnityEngine;

namespace StaticConfig.Units {
  [CreateAssetMenu(menuName = "Units/ExhaustibleResourceTracker")]
  public class ExhaustibleResourceTracker : ScriptableObject {
    public ExhaustibleResource exhaustibleResource;
    public int max;
    public int min;
    public int current;

    public void Reset() {
      current = max;
    }

    public void NewRound() {
      if (exhaustibleResource.renewsOnNewRound) {
        Reset();
      }
    }

    public void Expend(int amount) {
      current = Math.Max(min, current - amount);
    }

    public static ExhaustibleResourceTracker NewHpTracker(int maxHp) {
      var result =
          Instantiate(Resources.Load<ExhaustibleResourceTracker>("HpTracker"));
      result.max = maxHp;
      result.current = maxHp;
      return result;
    }
    
    public static ExhaustibleResourceTracker NewActionPointsTracker(int maxHp) {
      var result =
          Instantiate(Resources.Load<ExhaustibleResourceTracker>("ApTracker"));
      result.max = maxHp;
      result.current = maxHp;
      return result;
    }
    
    public static ExhaustibleResourceTracker NewMovementTracker(int maxHp) {
      var result =
          Instantiate(Resources.Load<ExhaustibleResourceTracker>("MpTracker"));
      result.max = maxHp;
      result.current = maxHp;
      return result;
    }
  }
}