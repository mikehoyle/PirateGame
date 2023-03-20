using Encounters;
using Events;
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
    public Option<UnitAbility> selectedAbility;
    public Vector3Int abilitySource;
    public Option<EncounterActor> selectedUnit;

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

    public void SelectAbility(PlayerUnitController actor, UnitAbility ability, Vector3Int? source = null) {
      selectedAbility = Option.Some(ability);
      abilitySource = source ?? actor.Position;
      Dispatch.Encounters.AbilitySelected.Raise(actor, ability, abilitySource);
    }

    public bool TryGet(out UnitAbility ability, out PlayerUnitController playerUnit) {
      if (selectedAbility.HasValue && selectedUnit.HasValue) {
        ability = selectedAbility.ValueOrFailure();
        var unit = selectedUnit.ValueOrFailure();
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
      if (selectedUnit.HasValue) {
        var untypedUnit = selectedUnit.ValueOrFailure();
        if (untypedUnit is T typedUnit) {
          unit = typedUnit;
          return true;
        }
      }

      unit = null;
      return false;
    }
    
    public void Clear() {
      selectedAbility = Option.None<UnitAbility>();
      selectedUnit = Option.None<EncounterActor>();
    }
  }
}