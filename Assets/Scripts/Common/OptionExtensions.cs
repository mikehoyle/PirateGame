using Optional;
using Optional.Unsafe;

namespace Common {
  public static class OptionExtensions {
    public static bool TryGet<T>(this Option<T> option, out T result) {
      if (option.HasValue) {
        result = option.ValueOrFailure();
        return true;
      }

      result = default(T);
      return false;
    }
  }
}