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
