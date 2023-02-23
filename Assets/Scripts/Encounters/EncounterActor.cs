using System;
using System.Collections.Generic;
using System.Linq;
using Common.Animation;
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
    [SerializeField] protected List<StatusEffect> activeStatusEffects;
    [SerializeField] protected EncounterEvents encounterEvents;
    [SerializeField] protected ExhaustibleResources exhaustibleResources;
    
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

    protected virtual void Awake() {
      _mover = GetComponent<UnitMover>();
      _collider = GetComponent<PolygonCollider2D>();
      AnimationState = AnimationNames.Idle;
    }

    protected virtual void OnEnable() {
      encounterEvents.applyAoeEffect.RegisterListener(OnApplyAoeEffect);
    }

    protected virtual void OnDisable() {
      encounterEvents.applyAoeEffect.UnregisterListener(OnApplyAoeEffect);
    }

    protected virtual void Update() {
      UpdateStatusEffects();
    }

    public void Init(UnitEncounterState encounterState) {
      EncounterState = encounterState;
      foreach (var passiveEffect in encounterState.metadata.passiveEffects ?? Enumerable.Empty<StatusEffect>()) {
        AddStatusEffect(passiveEffect.Apply(this));
      }
      ApplySize(encounterState.metadata.size);
      InitInternal(encounterState);
    }

    private void ApplySize(Vector2Int size) {
      _collider.offset = new Vector2(0, -SceneTerrain.CellHeightInWorldUnits / 2);
      _collider.SetPath(0, new Vector2[] {
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(0, 0)),
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(0, size.y)),
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(size.x, size.y)),
          SceneTerrain.CellBaseWorldStatic(new Vector3Int(size.x, 0)),
      });
    }

    protected virtual void InitInternal(UnitEncounterState encounterState) { }

    private void UpdateStatusEffects() {
      for (int i = activeStatusEffects.Count - 1; i >= 0; i--) {
        if (activeStatusEffects[i].UpdateAndMaybeDestroy(this)) {
          activeStatusEffects.RemoveAt(i);
        }
      }
    }

    private void OnApplyAoeEffect(AreaOfEffect aoe, StatusEffect effect) {
      if (aoe.AffectsPoint(Position)) {
        AddStatusEffect(effect);
      }
    }

    public void PlayOneOffAnimation(string animationName) {
      OneOffAnimation?.Invoke(animationName);
    }

    public List<UnitAbility> GetAllCapableAbilities() {
      return EncounterState.metadata.GetAbilities();
    }

    public void FaceTowards(Vector3Int target) {
      var directionVector = (Vector2Int)target - (Vector2Int)Position;
      // Strategy: choose the "dominant" difference in direction, and use that to determine facing. If the x and y
      // diff are equal, it's a diagonal (wash), so prefer the more negative of the two (i.e. facing camera).
      bool preferX = directionVector switch {
          { x: var x, y: var y } when Math.Abs(x) > Math.Abs(y) => true,
          { x: var x, y: var y } when Math.Abs(x) < Math.Abs(y) => false,
          { x: var x, y: var y } when x < y => true,
          _ => false,
      };

      if (preferX) {
        if (directionVector.x > 0) {
          FacingDirection = FacingDirection.NorthEast;
          return;
        }
        FacingDirection = FacingDirection.SouthWest;
        return;
      }
      if (directionVector.y > 0) {
        FacingDirection = FacingDirection.NorthWest;
        return;
      }
      FacingDirection = FacingDirection.SouthEast;
    }

    public void MoveAlongPath(TravelPath path, Action callback) {
      if (!path.IsViable()) {
        callback();
        return;
      }
      _mover.ExecuteMovement(path.Path, callback);
    }

    public void AddStatusEffect(StatusEffect effect) {
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