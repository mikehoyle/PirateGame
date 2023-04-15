﻿using Common.Loading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace HUD.ShipManagement {
  public class BackToMapButton : MonoBehaviour {
    private Button _button;

    private void Awake() {
      _button = GetComponent<Button>();
    }

    private void OnEnable() {
      _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable() {
      _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick() {
      SceneLoader.Instance.LoadScene(SceneId.Overworld);
    }
  }
}