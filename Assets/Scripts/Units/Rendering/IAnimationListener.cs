namespace Units.Rendering {
  public interface IAnimationListener {
    void OnNewFrame(int frameIndex, bool mirror);
    void OnAnimationComplete(CompositeAnimation.Type animationType);
  }
}