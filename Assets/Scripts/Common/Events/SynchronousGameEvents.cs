using System.Collections;

namespace Common.Events {
  public class SynchronousGameEvents {
    public class SynchronousGameEvent : BaseGameEvent<SynchronousGameEvent.OnEventRaised> {
      public delegate IEnumerator OnEventRaised();

      public IEnumerator Raise() {
        for (int i = Listeners.Count - 1; i >= 0; i--) {
          yield return Listeners[i].Invoke();
        }
      }
    }

    public class SynchronousGameEvent<T1> : BaseGameEvent<SynchronousGameEvent<T1>.OnEventRaised> {
      public delegate IEnumerator OnEventRaised(T1 param);

      public IEnumerator Raise(T1 param) {
        for (int i = Listeners.Count - 1; i >= 0; i--) {
          yield return Listeners[i].Invoke(param);
        }
      }
    }
  
    public class SynchronousGameEvent<T1, T2> : BaseGameEvent<SynchronousGameEvent<T1, T2>.OnEventRaised> {
      public delegate IEnumerator OnEventRaised(T1 param1, T2 param2);

      public IEnumerator Raise(T1 param1, T2 param2) {
        for (int i = Listeners.Count - 1; i >= 0; i--) {
          yield return Listeners[i].Invoke(param1, param2);
        }
      }
    }
  
    public class SynchronousGameEvent<T1, T2, T3> : BaseGameEvent<SynchronousGameEvent<T1, T2, T3>.OnEventRaised> {
      public delegate IEnumerator OnEventRaised(T1 param1, T2 param2, T3 param3);

      public IEnumerator Raise(T1 param1, T2 param2, T3 param3) {
        for (int i = Listeners.Count - 1; i >= 0; i--) {
          yield return Listeners[i].Invoke(param1, param2, param3);
        }
      }
    }
  }
}