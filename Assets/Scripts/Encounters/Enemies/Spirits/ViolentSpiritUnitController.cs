using System.Collections;
using System.Collections.Generic;
using Encounters.Effects;
using Events;
using Optional;
using State.Unit;
using Units;
using Units.Abilities.AOE;
using Units.Abilities.FX;
using UnityEngine;

namespace Encounters.Enemies.Spirits {
  public class ViolentSpiritUnitController : SpiritUnitController {
    [SerializeField] private int explosionDamage;
    [SerializeField] [Multiline] private string explosionAoe;
    [SerializeField] protected AbilityCastEffect explosionFx;
    
    private AreaOfEffect _areaOfEffect;

    private void Awake() {
      UpdateAoeDefinition();
    }

    private void OnValidate() {
      UpdateAoeDefinition();
    }

    private void UpdateAoeDefinition() {
      if (explosionAoe == null) {
        return;
      }
      _areaOfEffect = AoeParser.ParseAreaOfEffect(explosionAoe);
    }
    
    protected override IEnumerator OnTargetBonesReached(Bones bones) {
      var affectedFactions = new List<UnitFaction> {
          UnitFaction.Enemy,
          UnitFaction.PlayerParty,
      };
      var damageEffect = OneShotStatusEffect.SimpleDamageEffect(explosionDamage);
      yield return explosionFx.Execute(
          Option.None<EncounterActor>(),
          bones.Position,
          bones.Position,
          Option.Some(_areaOfEffect),
          () => {
            var instanceFactory = new StatusEffectApplier(damageEffect, affectedFactions);
            Dispatch.Encounters.ApplyAoeEffect.Raise(_areaOfEffect, instanceFactory);
          },
          () => { }
      );
    }
  }
}