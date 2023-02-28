using Common;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainTitle {
  public class TitleSceneManager : MonoBehaviour {
    private NewGameCreator _newGameCreator;

    private void Awake() {
      _newGameCreator = GetComponent<NewGameCreator>();
    }

    private void Start() {
      _newGameCreator.SetUpNewGame();
    }

    public void OnPlayButtonClick() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}