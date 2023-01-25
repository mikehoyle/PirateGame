using System;
using UnityEngine;

namespace State {
  [Serializable]
  public class UnitState {
    public Vector3Int Position;
    public int MaxHp;
    public int CurrentHp;
    public UnitControlSource ControlSource;
    public UnitFaction Faction;
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