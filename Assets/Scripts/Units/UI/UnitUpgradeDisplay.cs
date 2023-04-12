using System.Collections.Generic;
using Encounters;
using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Units.UI {
  public class UnitUpgradeDisplay : MonoBehaviour {
    private VisualElement _root;
    private TextElement _unitName;
    private Camera _unitCamera;
    private List<VisualElement> _tabs;
    private VisualElement _content;

    private void Awake() {
      _root = GetComponent<UIDocument>().rootVisualElement;
      _unitCamera = GetComponentInChildren<Camera>();
      SetVisible(false);
    }

    private void OnEnable() {
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.RegisterListener(OnCloseCharacterSheet);
    }

    private void OnDisable() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(OnOpenCharacterSheet);
      Dispatch.ShipBuilder.CloseCharacterSheet.UnregisterListener(OnCloseCharacterSheet);
    }

    private void Start() {
      _unitName = _root.Q<TextElement>("ChampionName");
      _tabs = _root.Query<VisualElement>(className: "panel-tab").ToList();
      for (int i = 0; i < _tabs.Count; i++) {
        var tabIndex = i;
        _tabs[i].RegisterCallback<ClickEvent>(_ => SetActiveTab(tabIndex));
      }

      _content = _root.Q<VisualElement>("UpgradesPanel");
    }

    private void OnOpenCharacterSheet(EncounterActor unit) {
      _unitName.text = unit.EncounterState.metadata.GetName();
      _unitCamera.transform.position = unit.transform.position + new Vector3(0, 0.25f, -10f);
      SetActiveTab(0);
      SetVisible(true);
    }

    private void OnCloseCharacterSheet() {
      SetVisible(false);
    }

    private void SetActiveTab(int tabIndex) {
      if (tabIndex < 0 || tabIndex > _tabs.Count) {
        Debug.LogWarning($"Attempted to activate tab outside tab range: {tabIndex}");
        return;
      }

      for (int i = 0; i < _tabs.Count; i++) {
        var tab = _tabs[i];
        if (i == tabIndex) {
          tab.AddToClassList("panel-tab-active");
          tab.RemoveFromClassList("panel-tab-inactive");
        } else {
          tab.RemoveFromClassList("panel-tab-active");
          tab.AddToClassList("panel-tab-inactive");
        }
      }
      
      // TODO IMMEDIATE update content for tab.
    }

    private void SetVisible(bool visible) {
      _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
  }
}