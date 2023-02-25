namespace Units.Abilities.Formulas.Values {
  public interface IDerivedValue {
    public float GetValue(UnitAbility.AbilityExecutionContext context, float skillTestResult);
    public string DisplayString();
  }
}
