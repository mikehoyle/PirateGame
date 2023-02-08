namespace Common.Events {
  public class ParameterizedGameEvent<T1> : GameEvent<ParameterizedGameEvent<T1>.OnEventRaised> {
    public delegate void OnEventRaised(T1 param);

    public void Raise(T1 param) {
      foreach (var listener in Listeners) {
        listener.Invoke(param);
      }
    }
  }
  
  public class ParameterizedGameEvent<T1, T2> : GameEvent<ParameterizedGameEvent<T1, T2>.OnEventRaised> {
    public delegate void OnEventRaised(T1 param1, T2 param2);

    public void Raise(T1 param1, T2 param2) {
      foreach (var listener in Listeners) {
        listener.Invoke(param1, param2);
      }
    }
  }
  
  public class ParameterizedGameEvent<T1, T2, T3> : GameEvent<ParameterizedGameEvent<T1, T2, T3>.OnEventRaised> {
    public delegate void OnEventRaised(T1 param1, T2 param2, T3 param3);

    public void Raise(T1 param1, T2 param2, T3 param3) {
      foreach (var listener in Listeners) {
        listener.Invoke(param1, param2, param3);
      }
    }
  }
}