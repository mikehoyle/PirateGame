using Common;
using HUD.MainMenu;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tavern {
  // TODO(P0): Almost all of the functionality here.
  public class TavernManager : MonoBehaviour {
    [SerializeField] private string backToMapButtonLabel = "Back to Map";
    private MainMenuController _mainMenu;
    
    private void Start() {
      _mainMenu = MainMenuController.Get();
      _mainMenu.AddMenuItem(backToMapButtonLabel, OnBackToMap);
    }
    
    private void OnBackToMap() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}