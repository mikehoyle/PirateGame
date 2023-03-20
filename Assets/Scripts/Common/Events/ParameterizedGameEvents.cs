namespace Common.Events {
  public class GameEvent : BaseGameEvent<GameEvent.OnEventRaised> {
    public delegate void OnEventRaised();

    public void Raise() {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        Listeners[i].Invoke();
      }
    }
  }

  public class GameEvent<T1> : BaseGameEvent<GameEvent<T1>.OnEventRaised> {
    public delegate void OnEventRaised(T1 param);

    public void Raise(T1 param) {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        Listeners[i].Invoke(param);
      }
    }
  }
  
  public class GameEvent<T1, T2> : BaseGameEvent<GameEvent<T1, T2>.OnEventRaised> {
    public delegate void OnEventRaised(T1 param1, T2 param2);

    public void Raise(T1 param1, T2 param2) {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        Listeners[i].Invoke(param1, param2);
      }
    }
  }
  
  public class GameEvent<T1, T2, T3> : BaseGameEvent<GameEvent<T1, T2, T3>.OnEventRaised> {
    public delegate void OnEventRaised(T1 param1, T2 param2, T3 param3);

    public void Raise(T1 param1, T2 param2, T3 param3) {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        Listeners[i].Invoke(param1, param2, param3);
      }
    }
  }
}