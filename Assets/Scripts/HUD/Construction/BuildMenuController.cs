using Events;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class BuildMenuController : MonoBehaviour {
    [SerializeField] private GameObject menuObjectPrefab;
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;

    private VerticalLayoutGroup _container;

    private void Awake() {
      gameObject.SetActive(false);
      _container = GetComponentInChildren<VerticalLayoutGroup>();
      Dispatch.ShipBuilder.EnterConstructionMode.RegisterListener(OnEnterConstruction);
      Dispatch.ShipBuilder.ExitConstructionMode.RegisterListener(OnExitConstruction);
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.EnterConstructionMode.UnregisterListener(OnEnterConstruction);
      Dispatch.ShipBuilder.ExitConstructionMode.UnregisterListener(OnExitConstruction);
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