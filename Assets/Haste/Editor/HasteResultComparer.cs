using System.Collections.Generic;
using UnityEditor;

namespace Haste {

  public class HasteResultComparer : IComparer<IHasteResult> {

    // -1: a is before than b
    // 0: a is around b
    // 1: a is after than b
    public int Compare(IHasteResult a, IHasteResult b) {
      // Order by score then fallback to length, then lexical ordering
      if (!HasteMathUtils.Approximately(a.Score, b.Score)) {
        return a.Score > b.Score ? -1 : 1;
      }

      // If scores are equal, order by path length
      if (a.Item.Path.Length != b.Item.Path.Length) {
        return a.Item.Path.Length < b.Item.Path.Length ? -1 : 1;
      }

      // If lengths are equal, order lexically
      return EditorUtility.NaturalCompare(a.Item.PathLower, b.Item.PathLower);
    }
  }
}
