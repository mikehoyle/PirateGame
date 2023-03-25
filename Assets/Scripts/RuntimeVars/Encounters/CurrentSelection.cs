using Encounters;
using Events;
using JetBrains.Annotations;
using Optional;
using Optional.Unsafe;
using Units;
using Units.Abilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/CurrentSelection")]
  public class CurrentSelection : ScriptableObject {
    public Option<UnitAbility> SelectedAbility { get; private set; }
    public Vector3Int AbilitySource { get; private set; }
    public Option<EncounterActor> SelectedUnit { get; private set; }

    private void Awake() {
      SceneManager.activeSceneChanged += OnSceneChanged;
#if UNITY_EDITOR
      UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
#endif
    }

    private void OnDestroy() {
      SceneManager.activeSceneChanged -= OnSceneChanged;
#if UNITY_EDITOR
      UnityEditor.SceneManagement.EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
#endif
    }

    private void OnSceneChanged(Scene a, Scene b) {
      Clear();
    }

    public void SelectUnit(EncounterActor unit) {
      if (SelectedUnit.Contains(unit)) {
        return;
      }

      SelectedUnit = unit.SomeNotNull();
      SelectedAbility = Option.None<UnitAbility>();
      Dispatch.Encounters.UnitSelected.Raise(unit);
    }

    public void SelectAbility(PlayerUnitController actor, UnitAbility ability, Vector3Int? source = null) {
      SelectedAbility = Option.Some(ability);
      AbilitySource = source ?? actor.Position;
      Dispatch.Encounters.AbilitySelected.Raise(actor, ability, AbilitySource);
    }

    public bool TryGet(out UnitAbility ability, out PlayerUnitController playerUnit) {
      if (SelectedAbility.HasValue && SelectedUnit.HasValue) {
        ability = SelectedAbility.ValueOrFailure();
        var unit = SelectedUnit.ValueOrFailure();
        if (unit is PlayerUnitController pUnit) {
          playerUnit = pUnit;
          return true;
        }
      }
      ability = null;
      playerUnit = null;
      return false;
    }

    public bool TryGetUnit<T>(out T unit) where T : EncounterActor {
      if (SelectedUnit.HasValue) {
        var untypedUnit = SelectedUnit.ValueOrFailure();
        if (untypedUnit is T typedUnit) {
          unit = typedUnit;
          return true;
        }
      }

      unit = null;
      return false;
    }
    
    public void Clear() {
      SelectedAbility = Option.None<UnitAbility>();
      SelectedUnit = Option.None<EncounterActor>();
      Dispatch.Encounters.UnitSelected.Raise(null);
    }
  }
}