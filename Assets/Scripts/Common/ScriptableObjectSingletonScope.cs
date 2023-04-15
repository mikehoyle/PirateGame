using UnityEngine;

namespace Common {
  /// <summary>
  /// A very stupid class that just holds all the scriptable object singletons in scope.
  /// </summary>
  public class ScriptableObjectSingletonScope : MonoBehaviour {
    public ScriptableObject[] singletons;
  }
}