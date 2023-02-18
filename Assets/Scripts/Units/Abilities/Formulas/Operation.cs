using System;

namespace Units.Abilities.Formulas {
  [Serializable]
  public enum Operation {
    Add = 0,
    Subtract = 1,
    Multiply = 2,
    Divide = 3,
  }
  
  
  public static class OperationExtensions {
    public static Func<float, float, float> GetOperation(this Operation operation) {
      return operation switch {
          Operation.Subtract => (a, b) => a - b,
          // If I divide by zero using this, it's my own damn fault.
          Operation.Divide => (a, b) => a / b,
          Operation.Multiply => (a, b) => a * b,
          // Default to Add
          _ => (a, b) => a + b,
      };
    }

    public static string DisplayString(this Operation operation) {
      return operation switch {
          Operation.Subtract => " - ",
          // If I divide by zero using this, it's my own damn fault.
          Operation.Divide => " / ",
          Operation.Multiply => " * ",
          // Default to Add
          _ => " + ",
      };
    }
  }
}