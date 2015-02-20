using System;

namespace Haste {

  public static class HasteArrayUtils {

    public static T[] Slice<T>(this T[] arr, int index) {
      int length = arr.Length - index;
      T[] result = new T[length];
      Array.Copy(arr, index, result, 0, length);
      return result;
    }

    public static T[] Slice<T>(this T[] arr, int index, int length) {
      T[] result = new T[length];
      Array.Copy(arr, index, result, 0, length);
      return result;
    }
  }
}
