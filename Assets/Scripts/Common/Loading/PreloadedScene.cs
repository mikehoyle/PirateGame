using UnityEngine;

namespace Common.Loading {
  public class PreloadedScene {
    private AsyncOperation _asyncOperation;

    public PreloadedScene(AsyncOperation asyncOperation) {
      _asyncOperation = asyncOperation;
    }
    
    public void Activate() {
      _asyncOperation.allowSceneActivation = true;
    }
  }
}