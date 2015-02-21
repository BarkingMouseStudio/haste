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
      indices.Sort();
      Indices = indices;
    }

    public float CalculateScore(string queryLower, out List<int> indices) {
      indices = new List<int>();

      string pathLower = Item.PathLower;
      int[] boundaryIndices = Item.BoundaryIndices;

      float score = 0;
      int pathIndex = 0;
      int queryIndex = 0;
      int boundaryPosition = 0;
      int pathLen = pathLower.Length;
      int queryLen = queryLower.Length;
      int boundaryLen = boundaryIndices.Length;
      int boundaryIndex;
      float gap = 0.0f;
      bool matchedChar = false;

      // 1. Favor name boundary matches (work backwards? match positions change around without look aheads)
      // 2. Favor boundary matches (still requires look aheads to ensure?)
      // 3. Penalize non-boundary match gaps
      // 4. Boost exact matches (no gap; might be implicit with 3)

      while (pathIndex < pathLen && queryIndex < queryLen) {
        matchedChar = false;

        // We matched a query character
        if (pathLower[pathIndex] == queryLower[queryIndex]) {

          // We have boundaries to match
          if (boundaryPosition < boundaryLen) {
            boundaryIndex = boundaryIndices[boundaryPosition];

            // Found a boundary match
            if (pathIndex == boundaryIndex) {
              score += 2.0f;
              boundaryPosition++;
              matchedChar = true;

            // Not a current boundary match
            } else {

              // This query character couldn't be matched on a future boundary
              if (pathLower[boundaryIndex] != queryLower[queryIndex]) {
                score += 1.0f / (gap + 1.0f);
                matchedChar = true;
              }
            }

          // Just a regular match
          } else {
            score += 1.0f / (gap + 1.0f);
            matchedChar = true;
          }
        }

        if (matchedChar) {
          indices.Add(pathIndex);
          queryIndex++;
          gap = 0;

        // Query and path characters don't match
        } else {
          // Increment gap between matched characters
          gap++;
        }

        // Advance to test next string
        pathIndex++;
      }

      if (pathLower.IndexOf("make parent") != -1) {
        HasteDebug.Info("{0} {1} {2}", Item.Path, Item.BoundariesLower, score);
      }

      return score;
    }

    public virtual bool Validate() {
      return true;
    }

    public virtual void Draw(bool isHighlighted) {
      using (new HasteVertical()) {
        EditorGUILayout.LabelField(Path.GetFileName(Item.Path), isHighlighted ? HasteStyles.HighlightedNameStyle : HasteStyles.NameStyle);
        EditorGUILayout.LabelField(HasteStringUtils.BoldLabel(Item.Path, Indices.ToArray(), isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.HighlightedDescriptionStyle : HasteStyles.DescriptionStyle);
      }
    }

    public virtual void Action() {
      // NO-OP
    }
  }
}
