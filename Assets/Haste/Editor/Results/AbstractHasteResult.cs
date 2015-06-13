using System;
using UnityEditor;
using UnityEngine;

namespace Haste {

  public abstract class AbstractHasteResult : IHasteResult {

    public IHasteItem Item { get; private set; }

    public float Score { get; private set; }

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
          int[] boundaryIndices = HasteStringUtils.GetBoundaryIndices(Item.Path);
          indices = HasteStringUtils.GetWeightedSubsequence(Item.PathLower, queryLower, boundaryIndices);
        }
        return indices;
      }
    }

    private readonly string queryLower;

    protected AbstractHasteResult(IHasteItem item, float score, string queryLower) {
      this.queryLower = queryLower;

      Score = score;
      Item = item;
    }

    // -1: a is before than b
    // 0: a is around b
    // 1: a is after than b
    public int CompareTo(IHasteResult b) {
      // Order by score then fallback to length, then lexical ordering
      if (!HasteMathUtils.Approximately(this.Score, b.Score)) {
        return this.Score > b.Score ? -1 : 1;
      }

      // If scores are equal, order by path length
      if (this.Item.Path.Length != b.Item.Path.Length) {
        return this.Item.Path.Length < b.Item.Path.Length ? -1 : 1;
      }

      // If lengths are equal, order lexically
      return EditorUtility.NaturalCompare(this.Item.PathLower, b.Item.PathLower);
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

    public virtual void Select() {
      if (HasteSettings.SelectEnabled) {
        Selection.activeObject = Object;
      }
    }

    public virtual void Action() {
      // NO-OP
    }
  }
}
