using Common;
using UnityEngine;

namespace StaticConfig.Units {
  [CreateAssetMenu(menuName = "Config/Stats")]
  public class Stats : ScriptableObjectSingleton<Stats> {
    public Stat strength;
    public Stat precision;
    public Stat innovation;
    public Stat constitution;
    public Stat movement;

    protected override Stats Self() {
      return this;
    }
  }
}