using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteResultComparer : IComparer<IHasteResult> {

    // Less than 0: a is less than b
    // Equals 0: a equals b
    // Greater than 0: a greater than b
    public int Compare(IHasteResult a, IHasteResult b) {
      // 1. Favor name boundaries
      // 2. Favor boundaries
      // 3. Penalize non-boundary gaps
      // 4. Boost exact matches
      if (a.Score != b.Score) {
        return a.Score < b.Score ? -1 : 1;
      }

      if (a.Item.Path.Length != b.Item.Path.Length) {
        return a.Item.Path.Length < b.Item.Path.Length ? -1 : 1;
      }

      return a.Item.PathLower.CompareTo(b.Item.PathLower);
    }
  }
}
