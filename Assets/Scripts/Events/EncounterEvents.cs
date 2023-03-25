using Common.Events;
using Encounters;
using Encounters.Effects;
using Optional;
using State.Encounter;
using State.Unit;
using Units;
using Units.Abilities;
using Units.Abilities.AOE;
using UnityEngine;

namespace Events {
  public class EncounterEvents {
    public readonly GameEvent<EncounterActor, UnitAbility> AbilityExecutionStart = new();
    public readonly GameEvent<EncounterActor, UnitAbility> AbilityExecutionEnd = new();
    public readonly GameEvent<int> TrySelectAbilityByIndex = new();
    public readonly GameEvent<PlayerUnitController, UnitAbility, Vector3Int> AbilitySelected = new();
    public readonly GameEvent ShipPlaced = new();
    public readonly GameEvent EncounterSetUp = new();
    public readonly GameEvent EncounterStart = new();
    public readonly GameEvent<EncounterOutcome> EncounterEnd = new();
    public readonly GameEvent EnemyTurnPreStart = new();
    public readonly GameEvent EnemyTurnStart = new();
    public readonly GameEvent EnemyTurnPreEnd = new();
    public readonly GameEvent EnemyTurnEnd = new();
    public readonly GameEvent PlayerTurnPreStart = new();
    public readonly GameEvent PlayerTurnStart = new();
    public readonly GameEvent<Vector3Int> MouseHover = new();
    public readonly GameEvent PlayerTurnEnd = new();
    public readonly GameEvent PlayerTurnEndRequest = new();
    public readonly GameEvent<EncounterActor> UnitSelected = new();
    public readonly GameEvent<AreaOfEffect, StatusEffectApplier> ApplyAoeEffect = new();
    public readonly GameEvent<EncounterActor> UnitAddedMidEncounter = new();
    public readonly GameEvent<Option<Bones>> UnitDeath = new();
    // Where int is rounds until spawn occurs
    public readonly GameEvent<UnitEncounterState, int> SpawnEnemyRequest = new();
    public readonly GameEvent<Bones> BonesCollected = new();
    public readonly GameEvent<CollectableInstance> ItemCollected = new();
    public readonly GameEvent<Vector3Int, CollectableInstance> SpawnCollectable = new();
  }
}