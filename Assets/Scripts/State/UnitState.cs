using System;
using UnityEngine;

namespace State {
  // TODO(P1): Create reasonable separation for far-less-capable enemy units or NPCs.
  [Serializable]
  public class UnitState {
    public Vector3Int StartingPosition;
    public int MaxHp;
    public UnitControlSource ControlSource;
    public UnitFaction Faction;
    public int MovementRange;
  }

  public enum UnitFaction {
    PlayerParty,
    Enemy,
  }

  public enum UnitControlSource {
    Player,
    AI,
  }
}