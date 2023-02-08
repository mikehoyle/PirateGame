using System;
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
    public List<UnitState> roster;
  }
}