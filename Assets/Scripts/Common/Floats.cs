namespace Common {
  /// <summary>
  /// Deal with common floating point precision concerns
  /// </summary>
  public static class Floats {
    private const float ErrorMargin = 0.001f;

    public static bool EqualsZero(float num) {
      return num is < ErrorMargin and > -ErrorMargin;
    }

    public static bool GreaterThanZero(float num) {
      return num > ErrorMargin;
    }

    public static bool LessThanZero(float num) {
      return num < -ErrorMargin;
    }
  }
}