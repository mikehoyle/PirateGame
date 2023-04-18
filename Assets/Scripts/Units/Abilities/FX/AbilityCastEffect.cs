using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.Animation;
using Common.Grid;
using Encounters;
using FMODUnity;
using Optional;
using State.Unit;
using Terrain;
using Units.Abilities.AOE;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Units.Abilities.FX {
  /// <summary>
  /// Represents the full config and execution for the visual & audio effects of an ability cast.
  ///
  /// Execution summary (sequential):
  ///  - Start PerformEffect immediately
  ///  - Wait completionDelaySecs seconds
  ///  - If provided, animate trail at designated speed.
  ///  - Perform impact effect.
  ///  - Wait impact animation duration.
  ///  - Done (call callback)
  /// </summary>
  [CreateAssetMenu(menuName = "Units/Abilities/AbilityCastEffect")]
  public class AbilityCastEffect : ScriptableObject {
    [Serializable]
    private class PerformEffect {
      public string casterAnimationName = "attack";
      public EventReference sound;
      public GameObject optionalParticlePrefab;
      public float particleHeight = 0.25f;
      public DirectionalAnimatedSprite optionalAnimation;
      public bool faceTarget = true;
      public float completionDelaySecs = 0.2f;
    }

    [Serializable]
    private class TransitEffect {
      public GameObject trailPrefab;
      [FormerlySerializedAs("createTrailsForEveryAoeTile")]
      public bool createTrailsForAoeTiles;
      public bool onlyCreateTrailsAtAffectedUnits;
      public float sourceHeight = 0.25f;
      public float targetHeight = 0.25f;
      public float arcHeight = 0.5f;
      public float speedTilesPerSec = 6;
      public EventReference sound;
    }

    [Serializable]
    private class ImpactEffect {
      [FormerlySerializedAs("createParticlesForEveryAoeTile")]
      public bool createParticlesForAoeTiles;
      public bool onlyCreateParticlesAtAffectedUnits;
      public GameObject optionalParticlePrefab;
      [FormerlySerializedAs("createAnimationForEveryAoeTile")]
      public bool createAnimationForAoeTiles; 
      public bool onlyCreateAnimationAtAffectedUnits;
      public DirectionalAnimatedSprite optionalAnimation;
      public float targetHeight = 0.25f;
      // TODO(P1): impact sound varies based on victim;
      public EventReference sound;
      public float animationDuration = 0.3f;
    }

    [SerializeField] private PerformEffect performEffect;
    [SerializeField] private TransitEffect transitEffect;
    [SerializeField] private ImpactEffect impactEffect;

    public IEnumerator Execute(
        UnitAbility.AbilityExecutionContext context,
        List<UnitFaction> affectedFactions,
        Option<AreaOfEffect> aoeOption,
        Action onImpactCallback,
        Action onCompleteCallback) {
      return Execute(
          Option.Some(context.Actor),
          affectedFactions,
          context.Source,
          context.TargetedTile,
          aoeOption,
          onImpactCallback,
          onCompleteCallback);
    }

    public IEnumerator Execute(
        Option<EncounterActor> actorOption,
        List<UnitFaction> affectedFactions,
        Vector3Int source,
        Vector3Int targetTile,
        Option<AreaOfEffect> aoeOption,
        Action onImpactCallback,
        Action onCompleteCallback) {
      //// PERFORM ////
      if (actorOption.TryGet(out var actor)) {
        if (performEffect.faceTarget && actor.Position != targetTile) {
          actor.FaceTowards(targetTile);
        }
        actor.PlayOneOffAnimation(performEffect.casterAnimationName);
      }

      MaybePlaySound(performEffect.sound);
      MaybeInstantiatePrefab(
          performEffect.optionalParticlePrefab, source, performEffect.particleHeight);
      MaybeCreateAnimation(performEffect.optionalAnimation, source, performEffect.particleHeight);
      yield return new WaitForSeconds(performEffect.completionDelaySecs);
      
      //// TRANSIT ////
      MaybePlaySound(transitEffect.sound);
      if (transitEffect.trailPrefab != null) {
        var targets = GetTargets(
            targetTile,
            aoeOption,
            transitEffect.createTrailsForAoeTiles,
            transitEffect.onlyCreateTrailsAtAffectedUnits,
            affectedFactions);
        var transitCoroutines = new List<Coroutine>();
        foreach (var target in targets) {
          var trail =
              MaybeInstantiatePrefab(transitEffect.trailPrefab, source, transitEffect.sourceHeight);
          if (trail.TryGetComponent<AbilityEffectTrail>(out var effectTrail)) {
            transitCoroutines.Add(effectTrail.StartCoroutine(
                effectTrail.ExecuteThenDie(
                    source,
                    transitEffect.sourceHeight,
                    target,
                    transitEffect.targetHeight,
                    transitEffect.arcHeight,
                    transitEffect.speedTilesPerSec)));
          } else {
            Debug.LogWarning("Trail object has no AbilityEffectTrail component");
          }
        }

        foreach (var coroutine in transitCoroutines) {
          yield return coroutine;
        }
      }
      
      //// IMPACT ////
      onImpactCallback();
      MaybePlaySound(impactEffect.sound);
      var particleTargets = GetTargets(
          targetTile,
          aoeOption,
          impactEffect.createParticlesForAoeTiles,
          impactEffect.onlyCreateParticlesAtAffectedUnits,
          affectedFactions);
      var animationTargets = GetTargets(
          targetTile,
          aoeOption,
          impactEffect.createAnimationForAoeTiles,
          impactEffect.onlyCreateAnimationAtAffectedUnits,
          affectedFactions);

      foreach (var particleTarget in particleTargets) {
        MaybeInstantiatePrefab(
            impactEffect.optionalParticlePrefab, particleTarget, impactEffect.targetHeight);
      }

      foreach (var animationTarget in animationTargets) {
        MaybeCreateAnimation(impactEffect.optionalAnimation, animationTarget, impactEffect.targetHeight);
      }

      yield return new WaitForSeconds(impactEffect.animationDuration);
      onCompleteCallback();
    }

    private List<Vector3Int> GetTargets(
        Vector3Int target,
        Option<AreaOfEffect> aoeOption,
        bool useAoe,
        bool restrictToAffectedUnits,
        List<UnitFaction> affectedFactions) {
      var targets = new List<Vector3Int>();
      if (useAoe && aoeOption.TryGet(out var aoe)) {
        targets = aoe.AffectedPoints().ToList();
        if (restrictToAffectedUnits) {
          targets = targets.Where(targetTile => {
            if (SceneTerrain.TryGetComponentAtTile<EncounterActor>(targetTile, out var actor)) {
              return affectedFactions.Contains(actor.EncounterState.faction);
            }
            return false;
          }).ToList();
        }
      } else {
        targets.Add(target);
      }
      return targets;
    }

    private GameObject MaybeInstantiatePrefab(GameObject particlePrefab, Vector3Int gridPosition, float height) {
      if (particlePrefab == null) {
        return null;
      }
      
      var result = Instantiate(particlePrefab);
      var position = GridUtils.CellCenterWorld(gridPosition);
      position.y += height;
      result.transform.position = position;
      return result;
    }

    private void MaybePlaySound(EventReference sound) {
      if (!sound.IsNull) {
        RuntimeManager.PlayOneShot(sound);
      }
    }
    
    private void MaybeCreateAnimation(DirectionalAnimatedSprite sprite, Vector3Int target, float targetHeight) {
      if (sprite == null) {
        return;
      }
      
      var impactObject = new GameObject("Impact Animation");
      impactObject.AddComponent<SpriteRenderer>();
      var animation = impactObject.AddComponent<EphemeralAnimation>();
      animation.PlayThenDie(target, targetHeight, sprite, "effect");
    }
  }
}