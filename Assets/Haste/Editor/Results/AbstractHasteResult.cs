using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Haste {

  public abstract class AbstractHasteResult : IHasteResult {

    public HasteItem Item { get; protected set; }
    public List<int> Indices { get; protected set; }
    public float Score { get; protected set; }

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

    public AbstractHasteResult(HasteItem item) {
      Item = item;

      // indexSum = 0;
      // gapSum = 0;
      Indices = new List<int>();
      Score = 0;
    }

    int LongestCommonSubsequenceLength(string first, string second) {
      string longer  = first.Length > second.Length ? first : second;
      string shorter = first.Length > second.Length ? second : first;

      int longerLen  = longer.Length;
      int shorterLen = shorter.Length;

      int[] previous = new int[shorterLen + 1];
      int[] current = new int[shorterLen + 1];

      for (int i = 0; i < longerLen; i++) {
        for (int j = 0; j < shorterLen; j++) {
          if (longer[i].ToUpper() == shorter[j].ToUpper()) {
            current[j + 1] = previous[j] + 1;
          } else {
            current[j + 1] = Math.Max(current[j], previous[j + 1]);
          }
        }

        for (int j = 0; j < shorterLen; j++) {
          previous[j + 1] = current[j + 1];
        }
      }

      return current[shorterLen];
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
