using System;
using UnityEngine;

namespace StaticConfig.Units {
  // Dead simple singleton
  [CreateAssetMenu(menuName = "Units/ExhaustibleResources")]
  public class ExhaustibleResources : ScriptableObject {
    public static ExhaustibleResources Instance { get; private set; }

    public ExhaustibleResource hp;
    public ExhaustibleResource ap;
    public ExhaustibleResource mp;

    private void OnEnable() {
      Instance = this;
    }

    private void OnDisable() {
      Instance = null;
    }
  }
}