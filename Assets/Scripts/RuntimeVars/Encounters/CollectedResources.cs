using System.Collections.Generic;
using Encounters;
using State.Encounter;
using UnityEngine;

namespace RuntimeVars.Encounters {
  /// <summary>
  /// Aggregates resources that have been collected in encounter (to later be cashed out if victory).
  /// </summary>
  [CreateAssetMenu(menuName = "Encounters/CollectedResources")]
  public class CollectedResources : ScriptableObject{
    [SerializeField] private List<CollectableInstance> collectedResources;

    private void Awake() {
      collectedResources = new();
    }

    public void GiveResourcesToPlayer() {
      foreach (var collectableInstance in collectedResources) {
        collectableInstance.AddToPlayerInventory();
      }
      Clear();
    }

    public void Clear() {
      collectedResources ??= new();
      collectedResources.Clear();
    }

    public void Add(CollectableInstance collectableInstance) {
      collectedResources.Add(collectableInstance);
    }
  }
}