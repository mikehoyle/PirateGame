﻿using System;
using System.Collections.Generic;
using StaticConfig;
using StaticConfig.Builds;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Construction {
  public class BuildMenuController : MonoBehaviour {
    [SerializeField] private GameObject menuObjectPrefab;
    [SerializeField] private AllBuildOptionsScriptableObject buildOptions;

    private VerticalLayoutGroup _container;
    private List<BuildMenuOption> _menuOptions = new();

    public event EventHandler<ConstructableObject> OnBuildSelected; 

    public ConstructableObject CurrentlySelectedItem { get; private set; }

    private void Awake() {
      _container = GetComponentInChildren<VerticalLayoutGroup>();
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

    private void OnBuildItemSelected(object _, ConstructableObject item) {
      Debug.Log($"Selected build item: {item.buildDisplayName}");
      CurrentlySelectedItem = item;
      OnBuildSelected?.Invoke(this, item);
    }

    public static BuildMenuController Get() {
      return GameObject.FindGameObjectWithTag(Tags.BuildMenu).GetComponent<BuildMenuController>();
    }
  }
}