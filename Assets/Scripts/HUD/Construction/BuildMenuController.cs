using System;
using System.Collections.Generic;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class BuildMenuController : MonoBehaviour {
    [SerializeField] private GameObject menuObjectPrefab;
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;

    private VerticalLayoutGroup _container;

    private void Awake() {
      _container = GetComponentInChildren<VerticalLayoutGroup>();
    }

    private void Start() {
      CreateMenuItems();
    }

    private void CreateMenuItems() {
      foreach (var buildOption in buildOptions.buildOptions) {
        var item = Instantiate(menuObjectPrefab, _container.transform).GetComponent<BuildMenuOption>();
        item.Init(buildOption);
      }
    }
  }
}