using State;
using StaticConfig;
using StaticConfig.RawResources;
using UnityEngine;
using UnityEngine.UI;

namespace HUD.Overworld {
  public class FoodIndicator : MonoBehaviour {
    [SerializeField] private RawResource foodResource;
    private Text _text;
    private const string Text = "Food: ";

    private void Start() {
      _text = GetComponent<Text>();
    }

    private void Update() {
      _text.text = Text + GameState.State.player.inventory.GetQuantity(foodResource);
    }
  }
}