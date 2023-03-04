using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Animation;
using Common.Events;
using Encounters.Effects;
using HUD.Encounter.HoverDetails;
using Optional;
using RuntimeVars.Encounters;
using RuntimeVars.Encounters.Events;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units;
using Units.Abilities;
using Units.Abilities.AOE;
using UnityEngine;

namespace Encounters {
  public abstract class EncounterActor : MonoBehaviour, IPlacedOnGrid, IDirectionalAnimatable, IDisplayDetailsProvider {
    [SerializeField] protected EncounterEvents encounterEvents;
    [SerializeField] protected CurrentSelection currentSelection;
    [SerializeField] protected ExhaustibleResources exhaustibleResources;
    
    private UnitMover _mover;
    private PolygonCollider2D _collider;
    private UnitShadow _shadow;

    public Vector3Int Position {
      get => EncounterState.position;
      set => EncounterState.position = value;
    }

    public FacingDirection FacingDirection {
      get => EncounterState.facingDirection;
      set => EncounterState.facingDirection = value;
    }
    
    public string AnimationState { get; set; }
    public event IDirectionalAnimatable.RequestOneOffAnimation OneOffAnimation;
    
    public GameObject StatusEffects { get; private set; }

    public abstract UnitEncounterState EncounterState { get; protected set; }
    protected abstract EmptyGameEvent TurnPreStartEvent { get; }

    protected virtual void Awake() {
      _mover = GetComponent<UnitMover>();
      _collider = GetComponent<PolygonCollider2D>();
      _shadow = GetComponentInChildren<UnitShadow>();
      AnimationState = AnimationNames.Idle;
      StatusEffects = new GameObject("Status Effects");
      StatusEffects.transform.parent = transform;
    }

    protected virtual void OnEnable() {
      encounterEvents.applyAoeEffect.RegisterListener(OnApplyAoeEffect);
      encounterEvents.objectClicked.RegisterListener(OnObjectClicked);
      TurnPreStartEvent.RegisterListener(PerformNewRoundSetup);
    }

    protected virtual void OnDisable() {
      encounterEvents.applyAoeEffect.UnregisterListener(OnApplyAoeEffect);
      encounterEvents.objectClicked.UnregisterListener(OnObjectClicked);
      TurnPreStartEvent.UnregisterListener(PerformNewRoundSetup);
    }

    public void Init(UnitEncounterState encounterState) {
      EncounterState = encounterState;
      foreach (var passiveEffect in encounterState.metadata.passiveEffects ?? Enumerable.Empty<StatusEffect>()) {
        passiveEffect.ApplyTo(this);
      }
      ApplySize(encounterState.metadata.size);
      InitInternal(encounterState);
    }

    private void PerformNewRoundSetup() {
      EncounterState.NewRound();
    }

    private void ApplySize(Vector2Int size) {
      _collider.offset = new Vector2(0, -SceneTerrain.CellHeightInWorldUnits / 2);
      _collider.SetPath(0, GridUtils.GetFootprintPolygon(size));
    }

    protected virtual void InitInternal(UnitEncounterState encounterState) { }

    private void OnApplyAoeEffect(AreaOfEffect aoe, StatusEffectApplier effect) {
      foreach (var occupiedTile in EncounterState.OccupiedTiles()) {
        if (aoe.AffectsPoint(occupiedTile)) {
          effect.ApplyTo(this);
          return;
        }
      }
    }
    
    private void OnObjectClicked(GameObject clickedObject) {
      if (clickedObject == gameObject) {
        if (currentSelection.selectedUnit.Contains(this)) {
          // Unit clicked but already selected, do nothing.
          return;
        }
        
        currentSelection.selectedUnit = Option.Some(this);
        currentSelection.selectedAbility = Option.None<UnitAbility>();
        encounterEvents.unitSelected.Raise(this);
      }
    }

    public void PlayOneOffAnimation(string animationName) {
      OneOffAnimation?.Invoke(animationName);
    }

    public List<UnitAbility> GetAllCapableAbilities() {
      return EncounterState.metadata.GetAbilities();
    }

    public void FaceTowards(Vector3Int target) {
      FacingDirection = FacingUtilities.DirectionBetween((Vector2Int)Position, (Vector2Int)target);
    }

    public Coroutine MoveAlongPath(TravelPath path) {
      if (!path.IsViable()) {
        return null;
      }
      return StartCoroutine(_mover.ExecuteMovement(path.Path));
    }

    public void DropIn(Action callback) {
      StartCoroutine(_mover.DropIn(callback));
    }

    public void EnableShadow(bool isEnabled) {
      _shadow.SetEnabled(isEnabled);
    }

    public void ExpendResource(ExhaustibleResource resource, int amount) {
      EncounterState.ExpendResource(resource, amount);
      if (EncounterState.GetResourceAmount(exhaustibleResources.hp) <= 0) {
        OnDeath();
        encounterEvents.unitDeath.Raise();
      }
    }

    protected abstract void OnDeath();

    public DisplayDetails GetDisplayDetails() {
      var additionalDetails = new List<string>();
      foreach (var resource in EncounterState.resources) {
        additionalDetails.Add(resource.DisplayString());
      }
      return new DisplayDetails {
          Name = EncounterState.metadata.GetName(),
          AdditionalDetails = additionalDetails,
      };
    }
  }
}