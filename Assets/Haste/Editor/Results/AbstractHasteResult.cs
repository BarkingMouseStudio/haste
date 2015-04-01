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
    public string NameBoundaries { get; private set; }

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
      NameBoundaries = HasteStringUtils.GetBoundaries(Name);

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
        EditorGUILayout.LabelField(String.Format("{0} {1}", Path.GetFileName(Item.Path), IsSelected ? HasteStyles.SelectionSymbol : ""), isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedName") : HasteStyles.Skin.GetStyle("Name"));

        // Description
        EditorGUILayout.LabelField(HasteStringUtils.BoldLabel(Item.Path, Indices, isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.Skin.GetStyle("HighlightedDescription") : HasteStyles.Skin.GetStyle("Description"));
      }
    }

    public virtual void Action() {
      // NO-OP
    }
  }
}
