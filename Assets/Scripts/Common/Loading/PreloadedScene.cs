namespace Common.Loading {
  public class PreloadedScene {
    private bool _activated;

    public PreloadedScene() {
      _activated = false;
    }
    
    public void Activate() {
      _activated = true;
    }

    public bool IsActivated() {
      return _activated;
    }
  }
}