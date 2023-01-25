using State;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Overworld {
  public class WaterIndicator : MonoBehaviour {
    private Text _text;
    private const string Text = "Water: ";

    private void Start() {
      _text = GetComponent<Text>();
    }

    private void Update() {
      _text.text = Text + GameState.State.Player.FoodQuantity;
    }
  }
}