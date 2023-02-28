using System.Collections.Generic;
using State.Unit;
using UnityEngine;

namespace State {
  /// <summary>
  /// Holds player-level game state to be saved. 
  /// </summary>
  [CreateAssetMenu(menuName = "State/PlayerState")]
  public class PlayerState : ScriptableObject {
    public Vector2Int overworldGridPosition;
    public ShipState ship;
    public InventoryState inventory;
    public ArmoryState armory;
    public List<PlayerUnitMetadata> roster;
    public int visionRange = 1;

    private PlayerState() {
      roster = new();
    }

    public PlayerState DeepCopy() {
      var copy = Instantiate(this);
      copy.ship = Instantiate(ship);
      copy.roster = new();

      foreach (var unit in roster) {
        copy.roster.Add(Instantiate(unit));
      }
      
      return copy;
    }
  }
}