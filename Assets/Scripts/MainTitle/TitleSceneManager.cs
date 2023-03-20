using System.Threading.Tasks;
using Common.Loading;
using UnityEngine;

namespace MainTitle {
  public class TitleSceneManager : MonoBehaviour {
    private NewGameCreator _newGameCreator;
    private PreloadedScene _overworldScene;

    private void Awake() {
      _newGameCreator = GetComponent<NewGameCreator>();
    }

    private void Start() {
      _newGameCreator.SetUpNewGame();
      _overworldScene = SceneLoader.Instance.PreloadScene(Scenes.Name.Overworld);
    }

    public void OnPlayButtonClick() {
      _overworldScene.Activate();
    }
  }
}