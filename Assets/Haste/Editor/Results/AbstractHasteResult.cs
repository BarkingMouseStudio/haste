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

    public float Score { get; private set; }

    public AbstractHasteResult(HasteItem item, string queryLower) {
      Item = item;

      List<int> indices;
      Score = CalculateScore(queryLower, out indices);
      Indices = indices;
    }

    public float CalculateScore(string queryLower, out List<int> indices) {
      float score = 0;
      HasteFuzzyMatching.FuzzyMatch(Item.Path, queryLower, out indices, out score);
      return score;
    }

    public List<int> GetIndices(string queryLower) {
      var indices = new List<int>();
      int lastIndex = 0;
      for (int i = 0; i < queryLower.Length; i++) {
        lastIndex = Item.PathLower.IndexOf(queryLower[i], lastIndex + 1);
        indices.Add(lastIndex);
      }
      return indices;
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
