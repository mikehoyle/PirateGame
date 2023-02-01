namespace Units {
  public enum UnitAction {
    None,
    Move,
    AttackMelee,
    EndTurn,
  }

  public static class UnitActionExtensions {
    public static string DisplayString(this UnitAction unitAction) => unitAction switch {
        UnitAction.None => "Do Nothing", // This should never be visible
        UnitAction.Move => "Move",
        UnitAction.AttackMelee => "Melee Attack",
        UnitAction.EndTurn => "End Turn",
        _ => "Unknown Action",
    };
  }
}