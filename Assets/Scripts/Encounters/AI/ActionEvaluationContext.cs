using Common;
using Encounters.Grid;
using RuntimeVars;
using RuntimeVars.Encounters;
using Terrain;

namespace Encounters.AI {
  public class ActionEvaluationContext {
    public SparseMatrix3d<bool> ClaimedTileOverrides { get; set; }
    public SceneTerrain Terrain { get; set; }
    public GridIndicators Indicators { get; set; }
    public EnemyUnitCollection EnemyUnits { get; set; }
    public UnitCollection PlayerUnits { get; set; }
  }
}