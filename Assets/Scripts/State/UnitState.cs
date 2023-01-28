using System;
using UnityEngine;

namespace State {
  // TODO(P1): Create reasonable separation for far-less-capable enemy units or NPCs.
  [Serializable]
  public class UnitState {
    public Vector3Int PositionOnShip;
    // TODO should encounter specifics not be in save files?
    public Vector3Int PositionInEncounter;
    public int MaxHp;
    public int CurrentHp;
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