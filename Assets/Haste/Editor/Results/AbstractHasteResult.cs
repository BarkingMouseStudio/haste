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
    public int[] Indices { get; private set; }
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


      // Try to match just the name first, the fall back to path matching
      string name = Path.GetFileNameWithoutExtension(Item.Path);
      int offset = Item.Path.LastIndexOf(name);
      int[] nameBoundaryIndices;
      HasteStringUtils.GetBoundaries(name, out nameBoundaryIndices).ToLower();

      // 1. Favor name boundary matches
      // 2. Favor boundary matches
      // 3. Penalize non-boundary match gaps
      // 4. Boost exact matches

      List<int> indices;
      float score;

      if (!CalculateScore(name, queryLower, offset, nameBoundaryIndices, out score, out indices)) {
        CalculateScore(Item.PathLower, queryLower, 0, Item.BoundaryIndices, out score, out indices);
      }

      Score = score;
      indices.Sort();
      Indices = indices.ToArray();
    }

    public bool CalculateScore(string pathLower, string queryLower, int offset, int[] boundaryIndices, out float score, out List<int> indices) {
      indices = new List<int>();
      score = 0;

      int pathLen = pathLower.Length;
      int queryLen = queryLower.Length;

      if (pathLen < queryLen) {
        // Can't match if the string is too short
        return false;
      }

      int pathIndex = 0;
      int queryIndex = 0;
      int boundaryPosition = 0;
      int boundaryLen = boundaryIndices.Length;
      int boundaryIndex;
      float gap = 0.0f;
      bool matchedChar = false;

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

            // No current boundary match, lookahead
            } else {
              bool couldMatchBoundary = false;
              int nextBoundaryIndex;

              while (boundaryPosition < boundaryLen) {
                nextBoundaryIndex = boundaryIndices[boundaryPosition];
                if (pathLower[nextBoundaryIndex] == queryLower[queryIndex]) {
                  couldMatchBoundary = true;
                  break;
                }
                boundaryPosition++;
              }

              // This query character couldn't be matched on a future boundary
              if (!couldMatchBoundary) {
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
          indices.Add(pathIndex + offset);
          queryIndex++;
          gap = 0;

          if (queryIndex > queryLower.Length - 1) {
            // If we have an exact match
            if (pathLower == queryLower) {
              // Bump the score by an extra point for each char
              score += queryLower.Length;
            }

            // We've reached the end of our query with successful matches
            return true;
          }

        // Query and path characters don't match
        } else {
          // Increment gap between matched characters
          gap++;
        }

        // Advance to test next string
        pathIndex++;
      }

      return false;
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
