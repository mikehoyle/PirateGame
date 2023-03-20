using System.Linq;
using Common;
using Encounters;
using Events;
using Optional.Collections;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Units;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.ShipManagement.CharacterSheet {
  public class StatRow : MonoBehaviour {
    [SerializeField] private Stat displayStat;
    [SerializeField] private CurrentSelection currentSelection;
    private Button _levelUpButton;

    private Text _text;

    private void Awake() {
      _text = GetComponentInChildren<Text>();
      _levelUpButton = GetComponentInChildren<Button>();
      _levelUpButton.onClick.AddListener(OnLevelUpClick);
      Dispatch.ShipBuilder.OpenCharacterSheet.RegisterListener(UpdateDisplay);
      Dispatch.ShipBuilder.UnitLevelUpStat.RegisterListener(UpdateDisplay);
    }

    private void Start() {
      _levelUpButton.gameObject.SetActive(false);
    }

    private void OnDestroy() {
      Dispatch.ShipBuilder.OpenCharacterSheet.UnregisterListener(UpdateDisplay);
      Dispatch.ShipBuilder.UnitLevelUpStat.UnregisterListener(UpdateDisplay);
    }

    private void UpdateDisplay(EncounterActor unit) {
      if (unit is not PlayerUnitController playerUnit) {
        return;
      }
      _levelUpButton.gameObject.SetActive(IsLevelUpAvailable(playerUnit.Metadata));
      foreach (var stat in playerUnit.Metadata.stats) {
        if (stat.stat == displayStat) {
          _text.text = $"{displayStat.displayName}: {stat.current}/{stat.stat.maxValue}\n";
          return;
        }
      }

      Debug.LogWarning($"Unit has does not have expected stat {displayStat.displayName}");
    }

    private bool IsLevelUpAvailable(PlayerUnitMetadata playerUnit) {
      var totalStatLevels = playerUnit.stats.Sum(statTracker => statTracker.current - 1);
      var canStatBeLeveled = playerUnit.stats
          .Where(statTracker => statTracker.stat == displayStat)
          .FirstOrNone()
          .Map(statTracker => statTracker.CanBeLeveledUp())
          .ValueOr(false);
      return (playerUnit.currentLevel > totalStatLevels + 1) && canStatBeLeveled;
    }

    private void OnLevelUpClick() {
      if (!currentSelection.selectedUnit.TryGet(out var unit)
          || unit.EncounterState.metadata is not PlayerUnitMetadata playerUnit
          || !IsLevelUpAvailable(playerUnit)) {
        return;
      }

      foreach (var stat in playerUnit.stats) {
        if (stat.stat == displayStat) {
          stat.LevelUp();
          Dispatch.ShipBuilder.UnitLevelUpStat.Raise((PlayerUnitController)unit);
          return;
        }
      }
      Debug.LogWarning($"Unit has does not have expected stat {displayStat.displayName}");
    }
  }
}