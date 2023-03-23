using System.Collections.Generic;
using System.Linq;
using StaticConfig.Encounters;
using UnityEngine;
using Random = System.Random;

namespace State.Unit {
  [CreateAssetMenu(menuName = "State/EnemyUnitTypeCollection")]
  public class EnemyUnitTypeCollection : ScriptableObject {
    [SerializeField] private float enemyDrModifier;
    public EnemyUnitMetadata[] enemyUnits;

    private readonly Random _rng;

    public EnemyUnitTypeCollection() {
      _rng = new Random();
    }

    public List<EnemyUnitMetadata> RandomEnemySpawnsForDifficulty(float difficultRating) {
      var result = new List<EnemyUnitMetadata>();
      var spawnVars = new EnemySpawnVariables {
          DifficultyRating = difficultRating,
      };

      // Maps displayName to spawn count.
      var spawnedEnemies = new Dictionary<string, int>();
      
      // Now pick the remainder of enemies by weighted chance.
      var remainingDr = difficultRating;
      while (remainingDr > 0) {
        var currentRemainingDr = remainingDr;
        var candidates = enemyUnits
            .Where(enemy => {
              spawnedEnemies.TryGetValue(enemy.displayName, out var spawnCount);
              return (enemy.spawnConfig.individualDifficultyRating * enemyDrModifier) <= currentRemainingDr
                  && spawnCount < enemy.spawnConfig.maxPerEncounter;
            })
            .ToList();
        var totalWeight = candidates.Sum(enemy => enemy.spawnConfig.GetSpawnWeight(spawnVars));
        Debug.Log($"Currently {candidates.Count} possible candidates with a total weight of {totalWeight}");
        var choice = _rng.NextDouble() * totalWeight;
        var chosenEnemy = candidates.FirstOrDefault(enemy => {
          choice -= enemy.spawnConfig.GetSpawnWeight(spawnVars);
          return choice <= 0;
        });

        if (chosenEnemy == null) {
          // No more enemies to satisfy DR, return what we have
          return result;
        }

        if (!spawnedEnemies.TryGetValue(chosenEnemy.displayName, out var count)) {
          spawnedEnemies[chosenEnemy.displayName] = 0;
        }
        spawnedEnemies[chosenEnemy.displayName] = count + 1;

        result.Add(chosenEnemy);
        remainingDr -= (chosenEnemy.spawnConfig.individualDifficultyRating * enemyDrModifier);
      }
      return result;
    }
  }
}