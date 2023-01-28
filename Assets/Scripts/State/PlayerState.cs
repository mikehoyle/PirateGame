using System;
using System.Collections.Generic;
using UnityEngine;

namespace State {
  /// <summary>
  /// Holds player-level game state to be saved. 
  /// </summary>
  [Serializable]
  public class PlayerState {
    public uint FoodQuantity;
    public uint WaterQuantity;
    public Vector2Int OverworldGridPosition = new(0, 0);
    public ShipState ShipState;
    public List<UnitState> Roster = new();
  }
}