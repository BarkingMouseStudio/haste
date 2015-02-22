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
      // if (a.IsFirstCharMatch != b.IsFirstCharMatch) {
      //   return a.IsFirstCharMatch ? -1 : 1;
      // }

      // bool equalBoundaryRatios = Mathf.Approximately(a.BoundaryQueryRatio, b.BoundaryQueryRatio);
      // bool equalBoundaryUtilization = Mathf.Approximately(a.BoundaryUtilization, b.BoundaryUtilization);

      // if (Mathf.Approximately(a.BoundaryQueryRatio, 1.0f) || Mathf.Approximately(b.BoundaryQueryRatio, 1.0f)) {
      //   if (!equalBoundaryRatios) {
      //     return a.BoundaryQueryRatio > b.BoundaryQueryRatio ? -1 : 1;
      //   } else if (!equalBoundaryUtilization) {
      //     return a.BoundaryUtilization > b.BoundaryUtilization ? -1 : 1;
      //   }
      // }

      // if (a.IsNamePrefixMatch != b.IsNamePrefixMatch) {
      //   return a.IsNamePrefixMatch ? -1 : 1;
      // }

      // if (a.IsPrefixMatch != b.IsPrefixMatch) {
      //   return a.IsPrefixMatch ? -1 : 1;
      // }

      // if (!equalBoundaryRatios) {
      //   return a.BoundaryQueryRatio > b.BoundaryQueryRatio ? -1 : 1;
      // } else if (!equalBoundaryUtilization) {
      //   return a.BoundaryUtilization > b.BoundaryUtilization ? -1 : 1;
      // }

      // if (a.IndexSum != b.IndexSum) {
      //   return a.IndexSum > b.IndexSum ? -1 : 1;
      // }

      // if (a.GapSum != b.GapSum) {
      //   return a.GapSum < b.GapSum ? -1 : 1;
      // }

      // if (a.PathLen != b.PathLen) {
      //   return a.PathLen < b.PathLen ? -1 : 1;
      // }

      // Try to order by score
      if (a.Score != b.Score) {
        return a.Score > b.Score ? -1 : 1;
      }

      // If scores are equal, order by length
      if (a.Item.Path.Length != b.Item.Path.Length) {
        return a.Item.Path.Length < b.Item.Path.Length ? -1 : 1;
      }

      // If lengths are equal, order lexically
      return EditorUtility.NaturalCompare(a.Item.PathLower, b.Item.PathLower);
    }
  }
}
