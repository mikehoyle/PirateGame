using UnityEngine;
using UnityEngine.InputSystem;

namespace HUD {
  public class CustomCursor : MonoBehaviour {
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private Texture2D cursorTexturePressed;

    private void Start() {
      Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    }

    private void Update() {
      var isPressed = Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed;
      Cursor.SetCursor(
          isPressed ? cursorTexturePressed : cursorTexture,
          Vector2.zero,
          CursorMode.Auto);
    }
  }
}