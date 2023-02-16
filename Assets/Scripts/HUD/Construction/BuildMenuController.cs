using RuntimeVars.ShipBuilder.Events;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class BuildMenuController : MonoBehaviour {
    [SerializeField] private GameObject menuObjectPrefab;
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;
    [SerializeField] private ShipBuilderEvents shipBuilderEvents;

    private VerticalLayoutGroup _container;

    private void Awake() {
      gameObject.SetActive(false);
      _container = GetComponentInChildren<VerticalLayoutGroup>();
      shipBuilderEvents.enterConstructionMode.RegisterListener(OnEnterConstruction);
      shipBuilderEvents.exitConstructionMode.RegisterListener(OnExitConstruction);
    }

    private void OnDestroy() {
      shipBuilderEvents.enterConstructionMode.UnregisterListener(OnEnterConstruction);
      shipBuilderEvents.exitConstructionMode.UnregisterListener(OnExitConstruction);
    }

    private void Start() {
      CreateMenuItems();
    }

    private void OnEnterConstruction() {
      gameObject.SetActive(true);
    }

    private void OnExitConstruction() {
      gameObject.SetActive(false);
    }

    private void CreateMenuItems() {
      foreach (var buildOption in buildOptions.buildOptions) {
        var item = Instantiate(menuObjectPrefab, _container.transform).GetComponent<BuildMenuOption>();
        item.Init(buildOption);
      }
    }
  }
}