using System;
using UnityEngine;

namespace Common {
  /// <summary>
  /// Intended to be subclassed by ScriptableObject types used as enums, where simple
  /// ID equality is used. 
  /// </summary>
  public abstract class EnumScriptableObject : ScriptableObject {
    [SerializeField] protected string id;

    // Equality overrides
    protected bool Equals(EnumScriptableObject other) {
      return id == other.id;
    }
    
    public override bool Equals(object obj) {
      if (ReferenceEquals(null, obj)) {
        return false;
      }
      if (ReferenceEquals(this, obj)) {
        return true;
      }
      if (obj.GetType() != this.GetType()) {
        return false;
      }
      return Equals((EnumScriptableObject)obj);
    }
    
    public override int GetHashCode() {
      return HashCode.Combine(id);
    }
  }
}