using Encounters;
using Events;
using UnityEngine;
using UnityEngine.UIElements;

namespace Units.UI {
  public class UnitUpgradeDisplay : MonoBehaviour {
    private VisualElement _root;
    private TextElement _unitName;
    private Camera _unitCamera;

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
    }

    private void OnOpenCharacterSheet(EncounterActor unit) {
      _unitName.text = unit.EncounterState.metadata.GetName();
      _unitCamera.transform.position = unit.transform.position + new Vector3(0, 0.25f, -10f);
      SetVisible(true);
    }

    private void OnCloseCharacterSheet() {
      SetVisible(false);
    }

    private void SetVisible(bool visible) {
      _root.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
    }
  }
}