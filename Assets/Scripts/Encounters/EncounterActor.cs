﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Animation;
using Common.Events;
using Encounters.Effects;
using RuntimeVars.Encounters.Events;
using State.Unit;
using StaticConfig.Units;
using Terrain;
using Units;
using Units.Abilities;
using Units.Abilities.AOE;
using UnityEngine;

namespace Encounters {
  public abstract class EncounterActor : MonoBehaviour, IPlacedOnGrid, IDirectionalAnimatable {
    [SerializeField] protected EncounterEvents encounterEvents;
    [SerializeField] protected ExhaustibleResources exhaustibleResources;
    [SerializeReference, SerializeReferenceButton]
    protected List<IStatusEffectInstance> activeStatusEffects;
    
    private UnitMover _mover;
    private PolygonCollider2D _collider;
    
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

    public abstract UnitEncounterState EncounterState { get; protected set; }
    protected abstract EmptyGameEvent TurnPreStartEvent { get; }

    protected virtual void Awake() {
      _mover = GetComponent<UnitMover>();
      _collider = GetComponent<PolygonCollider2D>();
      AnimationState = AnimationNames.Idle;
    }

    protected virtual void OnEnable() {
      encounterEvents.applyAoeEffect.RegisterListener(OnApplyAoeEffect);
      TurnPreStartEvent.RegisterListener(PerformNewRoundSetup);
    }

    protected virtual void OnDisable() {
      encounterEvents.applyAoeEffect.UnregisterListener(OnApplyAoeEffect);
      TurnPreStartEvent.UnregisterListener(PerformNewRoundSetup);
    }

    protected virtual void Update() {
      UpdateStatusEffects();
    }

    public void Init(UnitEncounterState encounterState) {
      EncounterState = encounterState;
      foreach (var passiveEffect in encounterState.metadata.passiveEffects ?? Enumerable.Empty<StatusEffect>()) {
        AddStatusEffect(passiveEffect.NewInstance(this));
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

    private void UpdateStatusEffects() {
      for (int i = activeStatusEffects.Count - 1; i >= 0; i--) {
        if (activeStatusEffects[i].UpdateAndMaybeDestroy(this)) {
          activeStatusEffects.RemoveAt(i);
        }
      }
    }

    private void OnApplyAoeEffect(AreaOfEffect aoe, StatusEffectInstanceFactory effect) {
      if (aoe.AffectsPoint(Position)) {
        AddStatusEffect(effect.GetInstance(this));
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

    public void MoveAlongPath(TravelPath path, Action callback) {
      if (!path.IsViable()) {
        callback();
        return;
      }
      _mover.ExecuteMovement(path.Path, callback);
    }

    public void AddStatusEffect(IStatusEffectInstance effect) {
      activeStatusEffects.Add(effect);
    }

    public void ExpendResource(ExhaustibleResource resource, int amount) {
      EncounterState.ExpendResource(resource, amount);
      if (EncounterState.GetResourceAmount(exhaustibleResources.hp) <= 0) {
        OnDeath();
        encounterEvents.unitDeath.Raise();
      }
    }

    protected abstract void OnDeath();
  }
}