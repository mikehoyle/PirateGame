using Encounters;
using Optional;
using Optional.Unsafe;
using Units.Abilities;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RuntimeVars.Encounters {
  [CreateAssetMenu(menuName = "Encounters/CurrentSelection")]
  public class CurrentSelection : ScriptableObject {
    public Option<UnitAbility> selectedAbility;
    public Option<EncounterActor> selectedUnit;

    private void Awake() {
      SceneManager.activeSceneChanged += OnSceneChanged;
#if UNITY_EDITOR
      EditorSceneManager.activeSceneChangedInEditMode += OnSceneChanged;
#endif
    }

    private void OnDestroy() {
      SceneManager.activeSceneChanged -= OnSceneChanged;
#if UNITY_EDITOR
      EditorSceneManager.activeSceneChangedInEditMode -= OnSceneChanged;
#endif
    }

    private void OnSceneChanged(Scene a, Scene b) {
      Clear();
    }

    public bool TryGet(out UnitAbility ability, out EncounterActor unit) {
      if (selectedAbility.HasValue && selectedUnit.HasValue) {
        ability = selectedAbility.ValueOrFailure();
        unit = selectedUnit.ValueOrFailure();
        return true;
      }
      ability = null;
      unit = null;
      return false;
    }
    
    public void Clear() {
      selectedAbility = Option.None<UnitAbility>();
      selectedUnit = Option.None<EncounterActor>();
    }
  }
}