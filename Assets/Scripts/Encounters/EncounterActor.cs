using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common.Animation;
using Common.Events;
using Common.Grid;
using Encounters.Effects;
using Events;
using HUD.Encounter.HoverDetails;
using Optional;
using RuntimeVars.Encounters;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units;
using Units.Abilities;
using Units.Abilities.AOE;
using UnityEngine;

namespace Encounters {
  public abstract class EncounterActor : MonoBehaviour, IPlacedOnGrid, IDirectionalAnimatable, IDisplayDetailsProvider {
    [SerializeField] protected CurrentSelection currentSelection;
    [SerializeField] protected GameObject bonesPrefab;
    [SerializeField] protected GameObject deathParticlesPrefab;

    protected UnitMover Mover;
    protected UnitDropIn DropInPerformer;
    private PolygonCollider2D _collider;
    private UnitShadow _shadow;

    public Vector3Int Position {
      get => EncounterState.position;
      set => EncounterState.position = value;
    }

    public bool BlocksAllMovement => true;
    public bool ClaimsTile => true;

    public FacingDirection FacingDirection {
      get => EncounterState.facingDirection;
      set => EncounterState.facingDirection = value;
    }

    public bool BlocksLineOfSight => false;
    
    public string AnimationState { get; set; }
    public event IDirectionalAnimatable.RequestOneOffAnimation OneOffAnimation;
    
    public GameObject StatusEffects { get; private set; }

    public abstract UnitEncounterState EncounterState { get; protected set; }
    protected abstract GameEvent TurnPreStartEvent { get; }
    protected abstract GameEvent TurnEndEvent { get; }

    protected virtual void Awake() {
      Mover = GetComponent<UnitMover>();
      DropInPerformer = GetComponent<UnitDropIn>();
      _collider = GetComponent<PolygonCollider2D>();
      _shadow = GetComponentInChildren<UnitShadow>();
      AnimationState = AnimationNames.Idle;
      StatusEffects = new GameObject("Status Effects");
      StatusEffects.transform.parent = transform;
    }

    protected virtual void OnEnable() {
      Dispatch.Encounters.ApplyAoeEffect.RegisterListener(OnApplyAoeEffect);
      Dispatch.Common.ObjectClicked.RegisterListener(OnObjectClicked);
      TurnEndEvent.RegisterListener(PerformNewRoundSetup);
    }

    protected virtual void OnDisable() {
      Dispatch.Encounters.ApplyAoeEffect.UnregisterListener(OnApplyAoeEffect);
      Dispatch.Common.ObjectClicked.UnregisterListener(OnObjectClicked);
      TurnEndEvent.UnregisterListener(PerformNewRoundSetup);
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
      EncounterState.NewRound(GetStat);
    }

    private void ApplySize(Vector2Int size) {
      _collider.offset = new Vector2(0, -GridUtils.CellHeightInWorldUnits / 2);
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
        currentSelection.SelectUnit(this);
      }
    }

    public void PlayOneOffAnimation(string animationName, Action onComplete) {
      OneOffAnimation?.Invoke(animationName, onComplete);
    }
    
    public void PlayOneOffAnimation(string animationName) {
      PlayOneOffAnimation(animationName, () => { });
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
      return StartCoroutine(Mover.ExecuteMovement(path.Path, true));
    }

    public void DropIn(Action callback) {
      EnableShadow(false);
      StartCoroutine(DropInPerformer.DropIn(() => {
        EnableShadow(true);
        callback();
      }));
    }

    private void EnableShadow(bool isEnabled) {
      _shadow.SetEnabled(isEnabled);
    }

    public int GetStat(Stat stat) {
      var value = EncounterState.metadata.GetStat(stat);
      foreach (GameObject statusEffect in StatusEffects.transform) {
        if (statusEffect.TryGetComponent<IStatModifier>(out var statModifier)) {
          value += statModifier.GetStatModifier(null, stat);
        }
      }
      return value;
    }

    public void ExpendResource(ExhaustibleResource resource, int amount) {
      EncounterState.ExpendResource(resource, amount);
      if (resource == ExhaustibleResources.Instance.hp && amount > 0) {
        PlayOneOffAnimation("flinch");
      }
      
      if (EncounterState.GetResourceAmount(ExhaustibleResources.Instance.hp) <= 0) {
        StartCoroutine(HandleDeath());
      }
    }

    private IEnumerator HandleDeath() {
      Instantiate(deathParticlesPrefab).transform.position = transform.position;
      OnDeath();

      var bonesOption = Option.None<Bones>();
      if (EncounterState.metadata.isRevivable) {
        var bones = Instantiate(bonesPrefab).GetComponent<Bones>();
        bones.Initialize(EncounterState, Position);
        bonesOption = Option.Some(bones);
      }

      var deathAnimationComplete = false;
      PlayOneOffAnimation("death", () => deathAnimationComplete = true);
      yield return new WaitUntil(() => deathAnimationComplete);
      
      Dispatch.Encounters.UnitDeath.Raise(bonesOption);
      Destroy(gameObject);
    }

    public int CurrentMovementRange() {
      return EncounterState.GetResourceAmount(ExhaustibleResources.Instance.mp);
    }

    public bool IsDead() {
      return EncounterState.GetResourceAmount(ExhaustibleResources.Instance.hp) <= 0;
    }

    protected abstract void OnDeath();

    public virtual DisplayDetails GetDisplayDetails() {
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