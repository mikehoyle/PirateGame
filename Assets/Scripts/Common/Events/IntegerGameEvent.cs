using UnityEngine;

namespace Common.Events {
  [CreateAssetMenu(menuName = "Events/IntegerGameEvent")]
  public class IntegerGameEvent : ParameterizedGameEvent<int> { }
}