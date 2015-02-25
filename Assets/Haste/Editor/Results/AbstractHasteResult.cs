using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haste {

  public abstract class AbstractHasteResult : IHasteResult {

    public HasteItem Item { get; private set; }
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
          indices = new int[0];
        }
        return indices;
      }
    }

    public bool IsFirstCharMatch { get; private set; }
    public bool IsFirstCharNameMatch { get; private set; }

    public bool IsPrefixMatch { get; private set; }
    public bool IsNamePrefixMatch { get; private set; }

    public bool IsExactMatch { get; private set; }
    public bool IsExactNameMatch { get; private set; }

    public float BoundaryQueryRatio { get; private set; }
    public float BoundaryUtilization { get; private set; }

    public AbstractHasteResult(HasteItem item, string queryLower, int queryLen) {
      Item = item;

      // The number of characters in the query that hit a boundary
      int boundaryMatchCount = HasteStringUtils.LongestCommonSubsequenceLength(queryLower, Item.BoundariesLower);

      BoundaryQueryRatio = boundaryMatchCount / queryLen;
      BoundaryUtilization = boundaryMatchCount / Item.BoundariesLower.Length;

      Name = HasteStringUtils.GetFileNameWithoutExtension(Item.Path);
      NameLower = Name.ToLowerInvariant();
      NameBoundaries = HasteStringUtils.GetBoundaries(Name);

      IsFirstCharMatch = Item.PathLower[0] == queryLower[0];
      IsFirstCharNameMatch = Name[0] == queryLower[0];

      // Much faster than "StartsWith"
      IsPrefixMatch = Item.PathLower.IndexOf(queryLower) == 0;
      IsNamePrefixMatch = NameLower.IndexOf(queryLower) == 0;

      IsExactMatch = Item.PathLower == queryLower;
      IsExactNameMatch = NameLower == queryLower;
    }

    public virtual bool Validate() {
      return true;
    }

    public virtual void Draw(bool isHighlighted, bool highlightMatches) {
      using (new HasteVertical()) {
        EditorGUILayout.LabelField(Path.GetFileName(Item.Path), isHighlighted ? HasteStyles.HighlightedNameStyle : HasteStyles.NameStyle);
        if (highlightMatches) {
          EditorGUILayout.LabelField(HasteStringUtils.BoldLabel(Item.Path, Indices, isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.HighlightedDescriptionStyle : HasteStyles.DescriptionStyle);
        } else {
          EditorGUILayout.LabelField(Item.Path, isHighlighted ? HasteStyles.HighlightedDescriptionStyle : HasteStyles.DescriptionStyle);
        }
      }
    }

    public virtual void Action() {
      // NO-OP
    }
  }
}
