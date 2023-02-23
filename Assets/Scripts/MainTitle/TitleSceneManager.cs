using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainTitle {
  public class TitleSceneManager : MonoBehaviour {
    public void OnPlayButtonClick() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}