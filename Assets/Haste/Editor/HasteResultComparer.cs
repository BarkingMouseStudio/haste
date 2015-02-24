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
      // - Favor name first
      // - Favor first char matches
      // - Favor boundary matches
      // - Favor prefix matches
      // - Penalize non-boundary match gaps
      // - Favor index sums
      // - Favor exact matches

      #if !IS_HASTE_PRO
      // Force menu item matches to the bottom in free version
      if (a.Item.Source != b.Item.Source) {
        return a.Item.Source == HasteMenuItemSource.NAME ? 1 : -1;
      }
      #endif

      // if (a.IsFirstCharMatch != b.IsFirstCharMatch || a.IsFirstCharNameMatch != b.IsFirstCharNameMatch) {
      //   return a.IsFirstCharMatch || a.IsFirstCharNameMatch ? -1 : 1;
      // }

      // bool equalBoundaryRatios = HasteUtils.Approximately(a.BoundaryQueryRatio, b.BoundaryQueryRatio);
      // bool equalBoundaryUtilization = HasteUtils.Approximately(a.BoundaryUtilization, b.BoundaryUtilization);

      // if (HasteUtils.Approximately(a.BoundaryQueryRatio, 1.0f) || HasteUtils.Approximately(b.BoundaryQueryRatio, 1.0f)) {
      //   if (!equalBoundaryRatios) {
      //     return a.BoundaryQueryRatio > b.BoundaryQueryRatio ? -1 : 1;
      //   } else if (!equalBoundaryUtilization) {
      //     return a.BoundaryUtilization > b.BoundaryUtilization ? -1 : 1;
      //   }
      // }

      // if (a.IsPrefixMatch != b.IsPrefixMatch || a.IsNamePrefixMatch != b.IsNamePrefixMatch) {
      //   return a.IsPrefixMatch || b.IsNamePrefixMatch ? -1 : 1;
      // }

      // if (!equalBoundaryRatios) {
      //   return a.BoundaryQueryRatio > b.BoundaryQueryRatio ? -1 : 1;
      // } else if (!equalBoundaryUtilization) {
      //   return a.BoundaryUtilization > b.BoundaryUtilization ? -1 : 1;
      // }

      // If scores are equal, order by path length
      // if (a.Item.Path.Length != b.Item.Path.Length) {
        return a.Item.Path.Length < b.Item.Path.Length ? -1 : 1;
      // }

      // // If lengths are equal, order lexically
      // return EditorUtility.NaturalCompare(a.Item.PathLower, b.Item.PathLower);
    }
  }
}
