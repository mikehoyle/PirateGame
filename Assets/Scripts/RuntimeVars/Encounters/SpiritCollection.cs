using System.Collections.Generic;
using Encounters.Enemies;
using UnityEngine;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/SpiritCollection")]
  public class SpiritCollection : ScriptableObject {
    public List<SpiritUnitController> spirits;

    public SpiritCollection() {
      spirits = new();
    }
  }
}