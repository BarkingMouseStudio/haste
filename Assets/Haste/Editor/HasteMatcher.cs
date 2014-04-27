using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteMatcher {

    readonly Regex boundaryRegex = new Regex(@"\b\w|[A-Z]");
    readonly string query;

    public HasteMatcher(string query) {
      this.query = query;
    }

    private bool Match(string path, string query, int multiplier, bool onlyBoundaries, out float score) {
      string queryLower = query.ToLower();
      string pathLower = path.ToLower();

      score = 0;

      if (pathLower.Length < queryLower.Length) {
        // Can't match if the string is too short
        return false;
      }

      int pathIndex = 0;
      int queryIndex = 0;
      int gap = 0;

      while (pathIndex < pathLower.Length) {
        bool matchedChar = false;

        if (pathLower[pathIndex] == queryLower[queryIndex]) {

          // Word Boundary
          if (boundaryRegex.Match(path, pathIndex, 1).Success) {
            score += 2 * multiplier;
            matchedChar = true;

          } else if (!onlyBoundaries) {
            // Try to match this query char on a boundary later, otherwise
            // we fall back to a sequential character match.
            float temp = 0;
            if (!Match(path.Substring(pathIndex + 1), query.Substring(queryIndex), 1, true, out temp)) {
              score += (1 / (gap + 1)) * multiplier;
              matchedChar = true;
            }
          }
        }

        if (matchedChar) {
          queryIndex++;

          if (queryIndex > queryLower.Length - 1) {
            // We've reached the end of our query with successful matches
            return true;
          }

          gap = 0;
        } else {
          gap++;
        }

        pathIndex++;

        if (pathLower.Length - pathIndex < queryLower.Length - queryIndex) {
          // Can't match if the remaining strings are too short
          break;
        }
      }

      return false;
    }

    public bool Match(string path, int multiplier, out float score) {
      return Match(path, query, multiplier, false, out score);
    }
  }
}
