using UnityEngine;

namespace StaticConfig.Units {
  [CreateAssetMenu(menuName = "Units/ExhaustibleResources")]
  public class ExhaustibleResources : ScriptableObject {
    public ExhaustibleResource hp;
    public ExhaustibleResource ap;
    public ExhaustibleResource mp;
  }
}