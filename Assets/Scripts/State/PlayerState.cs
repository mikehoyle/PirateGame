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
    public Vector2Int OverworldPosition = new(0, 0);
    public List<UnitState> Roster = new();
  }
}