using System;
using System.Collections.Generic;
using State;
using StaticConfig;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class BuildMenuController : MonoBehaviour {
    [SerializeField] private GameObject menuObjectPrefab;
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;

    private VerticalLayoutGroup _container;
    private GameState _gameState;
    private List<BuildMenuOption> _menuOptions = new();

    public event EventHandler<ConstructableScriptableObject> OnBuildSelected; 

    public ConstructableScriptableObject CurrentlySelectedItem { get; private set; }

    private void Awake() {
      _container = GetComponentInChildren<VerticalLayoutGroup>();
      _gameState = GameState.State;
    }

    private void Start() {
      CreateMenuItems();
    }

    private void OnDestroy() {
      foreach (var menuOption in _menuOptions) {
        menuOption.OnBuildOptionSelected -= OnBuildItemSelected;
      }
    }

    private void CreateMenuItems() {
      foreach (var buildOption in buildOptions.buildOptions) {
        var item = Instantiate(menuObjectPrefab, _container.transform).GetComponent<BuildMenuOption>();
        item.Init(buildOption);
        item.OnBuildOptionSelected += OnBuildItemSelected;
      }
    }

    private void OnBuildItemSelected(object _, ConstructableScriptableObject item) {
      Debug.Log($"Selected build item: {item.buildDisplayName}");
      CurrentlySelectedItem = item;
      OnBuildSelected?.Invoke(this, item);
    }

    public static BuildMenuController Get() {
      return GameObject.FindGameObjectWithTag(Tags.BuildMenu).GetComponent<BuildMenuController>();
    }
  }
}