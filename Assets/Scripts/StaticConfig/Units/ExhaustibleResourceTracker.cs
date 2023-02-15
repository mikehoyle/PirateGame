using System;
using UnityEngine;

namespace StaticConfig.Units {
  [Serializable]
  public class ExhaustibleResourceTracker {
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

    public static ExhaustibleResourceTracker NewTracker(ExhaustibleResource resource, int max) {
      return new ExhaustibleResourceTracker() {
          exhaustibleResource = resource,
          max = max,
          current = max,
      };
    }
  }
}