using System;

namespace StaticConfig.Units {
  [Serializable]
  public class ExhaustibleResourceTracker {
    public ExhaustibleResource exhaustibleResource;
    public GetResourceMax getResourceFunc;
    public int max;
    public int min;
    public int current;

    public delegate int GetResourceMax(Func<Stat, int> statGetter);

    public void Reset() {
      current = max;
    }

    public void NewRound(Func<Stat, int> statGetter) {
      var newMax = getResourceFunc(statGetter);
      if (newMax > max) {
        current += (newMax - max);
      }
      max = newMax;
      if (current > max) {
        current = max;
      }
      if (exhaustibleResource.renewsOnNewRound) {
        Reset();
      }
    }

    public void Expend(int amount) {
      current = Math.Min(max, Math.Max(min, current - amount));
    }

    public string DisplayString() {
      return $"{exhaustibleResource.displayName}: {current}/{max}";
    }

    public static ExhaustibleResourceTracker NewTracker(
        ExhaustibleResource resource, GetResourceMax getResourceFunction, Func<Stat, int> statGetter) {
      var maxStat = getResourceFunction(statGetter);
      return new ExhaustibleResourceTracker {
          exhaustibleResource = resource,
          getResourceFunc = getResourceFunction,
          max = maxStat,
          current = maxStat,
      };
    }
  }
}