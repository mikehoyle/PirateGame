using Common;
using UnityEngine;

namespace StaticConfig.Units {
  // Dead simple singleton
  [CreateAssetMenu(menuName = "Units/ExhaustibleResources")]
  public class ExhaustibleResources : ScriptableObjectSingleton<ExhaustibleResources> {
    public ExhaustibleResource hp;
    public ExhaustibleResource ap;
    public ExhaustibleResource mp;

    protected override ExhaustibleResources Self() {
      return this;
    }
  }
}