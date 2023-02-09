using System;
using System.Collections.Generic;
using System.Linq;
using Common.Events;
using JetBrains.Annotations;
using Optional;
using Pathfinding;
using RuntimeVars;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Unit;
using Units.Abilities;
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
    [SerializeField] private EmptyGameEvent endEnemyTurnEvent;
    [SerializeField] private EmptyGameEvent endAbilityExecutionEvent;
    [SerializeField] private CurrentSelection currentSelection;
    
    [CanBeNull] private UnitPlacementManager _placementManager;
    public Vector3Int Position {
      get => State.encounterState.position;
      set => State.encounterState.position = value;
    }
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
      endEnemyTurnEvent.RegisterListener(OnNewRound);
      endAbilityExecutionEvent.RegisterListener(OnAbilityEndExecution);
    }

    private void OnDisable() {
      playerUnitsInEncounter.Remove(this);
      objectClickedEvent.UnregisterListener(OnObjectClicked);
      endEnemyTurnEvent.UnregisterListener(OnNewRound);
      endAbilityExecutionEvent.UnregisterListener(OnAbilityEndExecution);
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
    public bool MoveAlongPath(TravelPath path, Action onCompleteCallback) {
      if (!path.IsViableAndWithinRange(State.encounterState.remainingMovement)) {
        return false;
      }
      
      _placementManager!.ExecuteMovement(path.Path, onCompleteCallback);
      return true;
    }

    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        if (currentSelection.selectedUnit.Contains(this)) {
          // Unit clicked but already selected, do nothing.
          return;
        }
        
        currentSelection.selectedUnit = Option.Some(this);
        unitSelectedEvent.Raise(this);
        TrySelectAbility(0);
      }
    }

    private void OnNewRound() {
      State.encounterState.NewRound(State);
    }

    private void OnAbilityEndExecution() {
      // If this unit is selected and just finished performing an ability,
      // Always go back to selecting the default ability
      if (currentSelection.selectedUnit.Contains(this)) {
        TrySelectAbility(0);
      }
    }
    public void TrySelectAbility(int index) {
      var abilities = GetAllCapableAbilities();
      if (abilities.Count <= index) {
        return;
      }

      var selectedAbility = abilities[index];
      currentSelection.selectedAbility = Option.Some(selectedAbility);
      abilitySelectedEvent.Raise(this, selectedAbility);
    }
  }
}