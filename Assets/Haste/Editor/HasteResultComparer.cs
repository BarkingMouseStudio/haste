using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteResultComparer : IComparer<IHasteResult> {

    // This is a shortcut instead of Mathf.Approximately which is slow.
    // We don't need perfect comparisons: its called "fuzzy matching".
    public static bool Approximately(float a, float b) {
      if (a > b) {
        return (a - b) < Mathf.Epsilon;
      } else if (a < b) {
        return (b - a) < Mathf.Epsilon;
      } else {
        return true;
      }
    }

    // -1: a is before than b
    // 0: a is around b
    // 1: a is after than b
    public int Compare(IHasteResult a, IHasteResult b) {
      #if !IS_HASTE_PRO
      // Force menu item matches to the bottom in free version
      if (a.Item.Source == HasteMenuItemSource.NAME || a.Item.Source == HasteLayoutSource.NAME) {
        return 1;
      } else if (b.Item.Source == HasteMenuItemSource.NAME || b.Item.Source == HasteLayoutSource.NAME) {
        return -1;
      }
      #endif

      // Favor exact path / name matches
      if (a.IsExactNameMatch != b.IsExactNameMatch) {
        return a.IsExactNameMatch ? -1 : 1;
      } else if (a.IsExactMatch != b.IsExactMatch) {
        return a.IsExactMatch ? -1 : 1;
      }

      // Favor prefix path / name matches
      if (a.IsNamePrefixMatch != b.IsNamePrefixMatch) {
        return a.IsNamePrefixMatch ? -1 : 1;
      } else if (a.IsPrefixMatch != b.IsPrefixMatch) {
        return a.IsPrefixMatch ? -1 : 1;
      }

      bool equalBoundaryRatios = Approximately(a.BoundaryQueryRatio, b.BoundaryQueryRatio);
      bool equalBoundaryUtilization = Approximately(a.BoundaryUtilization, b.BoundaryUtilization);

      // If boundary ratio / utilization approach 1:1, order by highest (favors full-matches)
      if (Approximately(a.BoundaryQueryRatio, 1.0f) || Approximately(b.BoundaryQueryRatio, 1.0f)) {
        if (!equalBoundaryRatios) {
          return a.BoundaryQueryRatio > b.BoundaryQueryRatio ? -1 : 1;
        } else if (!equalBoundaryUtilization) {
          return a.BoundaryUtilization > b.BoundaryUtilization ? -1 : 1;
        }
      }

      // Favor name matches by checking if the first char matches
      if (a.IsFirstCharNameMatch != b.IsFirstCharNameMatch) {
        return a.IsFirstCharNameMatch ? -1 : 1;
      } else if (a.IsFirstCharMatch != b.IsFirstCharMatch) {
        return a.IsFirstCharMatch ? -1 : 1;
      }

      // If boundary ratio / utilization don't match, order by highest (favors highest)
      if (!equalBoundaryRatios) {
        return a.BoundaryQueryRatio > b.BoundaryQueryRatio ? -1 : 1;
      } else if (!equalBoundaryUtilization) {
        return a.BoundaryUtilization > b.BoundaryUtilization ? -1 : 1;
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