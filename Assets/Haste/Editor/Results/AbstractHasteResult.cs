using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Haste {

  public abstract class AbstractHasteResult : IHasteResult {

    public HasteItem Item { get; private set; }
    public List<int> Indices { get; private set; }
    public float Score { get; private set; }

    public virtual bool IsDraggable {
      get { return false; }
    }

    public virtual float Height(bool isHighlighted) {
      return HasteStyles.ItemHeight;
    }

    public virtual UnityEngine.Object Object {
      get { return null; }
    }

    public virtual string DragLabel {
      get { return ""; }
    }

    public virtual bool IsSelected {
      get {
        return HasteWindow.Instance.IsSelected(Object);
      }
    }

    public AbstractHasteResult(HasteItem item, string queryLower) {
      Item = item;

      List<int> indices;
      Score = CalculateScore(queryLower, out indices);
      Indices = indices;
    }

    public float CalculateScore(string queryLower, out List<int> indices) {
      indices = new List<int>(0);

      float score = 0;
      // int gap = 0;

      // The number of characters in the query that hit a boundary
      int boundaryMatchCount = HasteStringUtils.LongestCommonSubsequenceLength(queryLower, Item.Boundaries);

      // The ratio of boundary characters in the query
      float boundaryQueryRatio = boundaryMatchCount / queryLower.Length;
      score += boundaryQueryRatio;

      // The ratio of matched boundary characters
      float boundaryUtilization = boundaryMatchCount / Item.Boundaries.Length;
      score += boundaryUtilization;

      // if (IsFirstCharNameMatch) {
      //   score += 2;
      // } else if (IsFirstCharMatch) {
      //   score += 1;
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

      // if (a.GapSum != b.GapSum) {
      //   return a.GapSum < b.GapSum ? -1 : 1;
      // }

      // if (a.pathLen != b.pathLen) {
      //   return a.pathLen < b.pathLen ? -1 : 1;
      // }

      // return a.Item.PathLower.CompareTo(b.Item.PathLower);

      return score;
    }

    public virtual bool Validate() {
      return true;
    }

    public virtual void Draw(bool isHighlighted) {
      using (new HasteVertical()) {
        EditorGUILayout.LabelField(Path.GetFileName(Item.Path), isHighlighted ? HasteStyles.HighlightedNameStyle : HasteStyles.NameStyle);
        EditorGUILayout.LabelField(HasteUtils.BoldLabel(Item.Path, Indices.ToArray(), isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.HighlightedDescriptionStyle : HasteStyles.DescriptionStyle);
      }
    }

    public virtual void Action() {
      // NO-OP
    }
  }
}
