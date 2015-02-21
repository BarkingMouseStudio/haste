using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Haste {

  public abstract class AbstractHasteResult : IHasteResult {

    public HasteItem Item { get; protected set; }
    public List<int> Indices { get; private set; }

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

    // TODO: Reduce property usage (fn calls) by implementing
    // a local `<` operator and using that in the IComparer.
    public bool IsFirstCharMatch { get; private set; }
    public bool IsPrefixMatch { get; private set; }
    public bool IsNamePrefixMatch { get; private set; }
    public int IndexSum { get; private set; }
    public int GapSum { get; private set; }
    public int PathLen { get; private set; }
    public string PathName { get; private set; }
    public float BoundaryQueryRatio { get; private set; }
    public float BoundaryUtilization { get; private set; }

    public AbstractHasteResult(HasteItem item, string query) {
      Item = item;

      string queryLower = query.ToLower();
      int boundaryMatchCount = HasteStringUtils.LongestCommonSubsequenceLength(queryLower, item.Boundaries);

      PathLen = item.Path.Length;
      BoundaryQueryRatio = boundaryMatchCount / query.Length;
      BoundaryUtilization = boundaryMatchCount / item.Boundaries.Length;
      IsNamePrefixMatch = Path.GetFileNameWithoutExtension(item.PathLower).StartsWith(queryLower);
      IsFirstCharMatch = item.PathLower[0] == queryLower[0];
      IsPrefixMatch = item.PathLower.StartsWith(queryLower);

      Indices = GetIndices();

      IndexSum = 0;
      foreach (int index in Indices) {
        IndexSum += index;
      }

      GapSum = 0;
      int a, b;
      for (int i = 1; i < Indices.Count; i++) {
        a = Indices[i - 1];
        b = Indices[i];
        GapSum += b - a;
      }
    }

    public List<int> GetIndices() {
      return new List<int>();
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
