using System.Collections;
using UnityEngine;

namespace Common.Events {
  public class SynchronousGameEvent : BaseGameEvent<SynchronousGameEvent.OnEventRaised> {
    public delegate IEnumerator OnEventRaised();

    public void Raise(MonoBehaviour runner) {
      runner.StartCoroutine(RaiseInner());
    }

    private IEnumerator RaiseInner() {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        yield return Listeners[i].Invoke();
      }
    }
  }

  public class SynchronousGameEvent<T1> : BaseGameEvent<SynchronousGameEvent<T1>.OnEventRaised> {
    public delegate IEnumerator OnEventRaised(T1 param);

    public void Raise(MonoBehaviour runner, T1 param) {
      runner.StartCoroutine(RaiseInner(param));
    }
    
    private IEnumerator RaiseInner(T1 param) {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        yield return Listeners[i].Invoke(param);
      }
    }
  }

  public class SynchronousGameEvent<T1, T2> : BaseGameEvent<SynchronousGameEvent<T1, T2>.OnEventRaised> {
    public delegate IEnumerator OnEventRaised(T1 param1, T2 param2);

    public void Raise(MonoBehaviour runner, T1 param1, T2 param2) {
      runner.StartCoroutine(RaiseInner(param1, param2));
    }
    
    private IEnumerator RaiseInner(T1 param1, T2 param2) {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        yield return Listeners[i].Invoke(param1, param2);
      }
    }
  }

  public class SynchronousGameEvent<T1, T2, T3> : BaseGameEvent<SynchronousGameEvent<T1, T2, T3>.OnEventRaised> {
    public delegate IEnumerator OnEventRaised(T1 param1, T2 param2, T3 param3);

    public void Raise(MonoBehaviour runner, T1 param1, T2 param2, T3 param3) {
      runner.StartCoroutine(RaiseInner(param1, param2, param3));
    }
    
    private IEnumerator RaiseInner(T1 param1, T2 param2, T3 param3) {
      for (int i = Listeners.Count - 1; i >= 0; i--) {
        yield return Listeners[i].Invoke(param1, param2, param3);
      }
    }
  }
}