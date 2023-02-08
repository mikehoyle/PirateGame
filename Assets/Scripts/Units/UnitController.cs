using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Optional;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Unit;
using StaticConfig.Units;
using Units.Rendering;
using UnityEngine;

namespace Units {
  public class UnitController : MonoBehaviour {
    [SerializeField] private float speedUnitsPerSec;
    [SerializeField] private UnitCollection playerUnitsInEncounter;
    [SerializeField] private UnitAbilitySet defaultAbilities;
    [SerializeField] private ObjectClickedEvent objectClickedEvent;
    [SerializeField] private UnitSelectedEvent unitSelectedEvent;
    [SerializeField] private AbilitySelectedEvent abilitySelectedEvent;
    [SerializeField] private CurrentSelection currentSelection;
    
    [CanBeNull] private UnitPlacementManager _placementManager;
    public Vector3Int Position { get; set; }
    public UnitState State { get; private set; }
    // TODO(P1): This doesn't seem to be the true center.
    public Vector3 WorldPosition => _placementManager!.GetPlacement();
    private AnimatedCompositeSprite _sprite;

    private void Awake() {
      var grid = GameObject.FindWithTag(Tags.Grid).GetComponent<Grid>();
      _sprite = GetComponentInChildren<AnimatedCompositeSprite>();
      _placementManager = new UnitPlacementManager(grid, this, _sprite, speedUnitsPerSec);
    }

    private void OnEnable() {
      playerUnitsInEncounter.Add(this);
      objectClickedEvent.RegisterListener(OnObjectClicked);
    }

    private void OnDisable() {
      playerUnitsInEncounter.Remove(this);
      objectClickedEvent.UnregisterListener(OnObjectClicked);
    }

    private void Start() {
      _sprite.SetColorForFaction(State.faction);
    }

    private void Update() {
      _placementManager!.Update();
      transform.position = WorldPosition;
    }

    public void Init(UnitState state) {
      Init(state, Vector3Int.zero);
    }

    public void Init(UnitState state, Vector3Int positionOffset) {
      State = state;
      state.encounterState.NewEncounter(state, positionOffset);
      Position = State.startingPosition + positionOffset;
      transform.position = WorldPosition;
    }

    public List<UnitAbility> GetAllCapableAbilities() {
      var result = defaultAbilities.abilities.ToList();
      result.AddRange(State.GetAbilities());
      return result;
    }
    
    /// <returns>Whether the unit is eligible to move along the path</returns>
    public bool MoveAlongPath(LinkedList<Vector3Int> path, Action onCompleteCallback) {
      if (!IsPathViable(path)) {
        return false;
      }
      
      _placementManager!.ExecuteMovement(path, onCompleteCallback);
      return true;
    }

    public static bool IsPathViable(LinkedList<Vector3Int> path) {
      return path != null && path.Count > 1;
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        if (currentSelection.selectedUnit.Contains(this)) {
          // Unit clicked but already selected, do nothing.
          return;
        }

        var selectedAbility = defaultAbilities.abilities[0];
        currentSelection.selectedUnit = Option.Some(this);
        currentSelection.selectedAbility = Option.Some(selectedAbility);
        unitSelectedEvent.Raise(this);
        abilitySelectedEvent.Raise(this, selectedAbility);
      }
    }
  }
}