using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haste {

  public abstract class AbstractHasteResult : IHasteResult {

    public IHasteItem Item { get; private set; }
    public string Name { get; private set; }
    public string NameLower { get; private set; }

    public bool IsVisible { get; set; }

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

    private int[] indices;
    public int[] Indices {
      get {
        if (indices == null) {
          indices = GetIndices();
        }
        return indices;
      }
    }

    int[] GetIndices() {
      int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(Item.Path);
      int[] indices = HasteStringUtils.GetWeightedSubsequence(Item.PathLower, QueryLower, boundaryIndices);
      return indices;
    }

    public bool IsFirstCharMatch { get; private set; }
    public bool IsFirstCharNameMatch { get; private set; }

    public bool IsPrefixMatch { get; private set; }
    public bool IsNamePrefixMatch { get; private set; }

    public bool IsExactMatch { get; private set; }
    public bool IsExactNameMatch { get; private set; }

    public float BoundaryQueryRatio { get; private set; }
    public float BoundaryUtilization { get; private set; }

    string QueryLower { get; set; }

    public AbstractHasteResult(IHasteItem item, string queryLower, int queryLen) {
      QueryLower = queryLower;
      Item = item;

      // The number of characters in the query that hit a boundary
      int boundaryMatchCount = HasteStringUtils.LongestCommonSubsequenceLength(queryLower, Item.BoundariesLower);
      BoundaryQueryRatio = boundaryMatchCount / queryLen;

      int boundaryLen = Item.BoundariesLower.Length;
      if (boundaryLen > 0) {
        BoundaryUtilization = boundaryMatchCount / boundaryLen;
      } else {
        BoundaryUtilization = 0;
      }

      Name = HasteStringUtils.GetFileNameWithoutExtension(Item.Path);
      NameLower = Name.ToLowerInvariant();

      IsFirstCharMatch = Item.PathLower[0] == queryLower[0];
      IsFirstCharNameMatch = NameLower[0] == queryLower[0];

      // Much faster than "StartsWith"
      IsPrefixMatch = queryLen >= 3 && Item.PathLower.IndexOf(queryLower) == 0;
      IsNamePrefixMatch = queryLen >= 3 && NameLower.IndexOf(queryLower) == 0;

      IsExactMatch = Item.PathLower == queryLower;
      IsExactNameMatch = NameLower == queryLower;
    }

    public virtual bool Validate() {
      return true;
    }

    public virtual void Draw(bool isHighlighted) {
      using (new HasteVertical()) {
        // Name
        var nameStyle = isHighlighted ?
          HasteStyles.GetStyle("HighlightedName") :
          HasteStyles.GetStyle("Name");
        EditorGUILayout.LabelField(HasteStringUtils.GetFileName(Item.Path), nameStyle);

        // Description
        var descriptionStyle = isHighlighted ?
          HasteStyles.GetStyle("HighlightedDescription") :
          HasteStyles.GetStyle("Description");
        var boldStart = isHighlighted ?
          HasteStyles.HighlightedBoldStart :
          HasteStyles.BoldStart;
        var description = HasteStringUtils.BoldLabel(Item.Path, Indices, boldStart, HasteStyles.BoldEnd);
        EditorGUILayout.LabelField(description, descriptionStyle);
      }
    }

    public virtual void Action() {
      // NO-OP
    }
  }
}
