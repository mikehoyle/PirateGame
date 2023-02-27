using System;
using Common;
using Overworld.MapGeneration;
using State;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MainTitle {
  public class TitleSceneManager : MonoBehaviour {
    private void Start() {
      GameState.State.world = new OverworldGenerator(width: 100, height: 100, seed: 1).GenerateWorld();
    }

    public void OnPlayButtonClick() {
      SceneManager.LoadScene(Scenes.Name.Overworld.SceneName());
    }
  }
}