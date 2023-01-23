using UnityEngine;

namespace Units {
  public class UnitController : MonoBehaviour {
    public bool IsMyTurn { get; set; }
    public UnitControlSource ControlSource { get; set; }
    public Faction Faction { get; set; }

    private void Awake() {
      IsMyTurn = false;
    }

    private void Update() {
      if (IsMyTurn) {
        HandleInput();
      }
    }
    
    private void HandleInput() {
      // TODO
    }

    public void Init(UnitControlSource controlSource, Faction faction) {
      IsMyTurn = false;
      ControlSource = controlSource;
      Faction = faction;
    }
  }

  public enum Faction {
    PlayerParty,
    Enemy,
  }

  public enum UnitControlSource {
    Player,
    AI,
  }
}