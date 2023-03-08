using State;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Overworld {
  public class CurrentDayIndicator : MonoBehaviour {
    private Text _text;
    private const string Text = "Day: ";

    private void Start() {
      _text = GetComponent<Text>();
    }

    private void Update() {
      //_text.text = Text + GameState.State.world.currentDay;
    }
  }
}