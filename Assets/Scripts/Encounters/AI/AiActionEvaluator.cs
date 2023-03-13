using Common;
using Encounters.Enemies;
using Encounters.Grid;
using RuntimeVars;
using RuntimeVars.Encounters;
using Terrain;
using UnityEngine;

namespace Encounters.AI {
  // TODO(P1): This is far too rudimentary, but it's what we've got for now.
  public class AiActionEvaluator : MonoBehaviour {
    [SerializeField] private UnitCollection playerUnits;
    [SerializeField] private EnemyUnitCollection enemyUnits;

    private SceneTerrain _terrain;
    private GridIndicators _indicators;

    private void Awake() {
      _terrain = SceneTerrain.Get();
      _indicators = GridIndicators.Get();
    }

    public AiActionPlan GetActionPlan(EnemyUnitController enemy, SparseMatrix3d<bool> claimedTileOverrides) {
      return enemy.GetActionPlan(new ActionEvaluationContext {
          ClaimedTileOverrides = claimedTileOverrides,
          Terrain = _terrain,
          Indicators = _indicators,
          EnemyUnits = enemyUnits,
          PlayerUnits = playerUnits,
      });
    }
  }
}