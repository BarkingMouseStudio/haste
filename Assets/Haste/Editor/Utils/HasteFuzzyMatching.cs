using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public static class HasteFuzzyMatching {

    static readonly Regex boundaryRegex = new Regex(@"\b\w|[A-Z]");

    static bool BoundaryMatch(string str, string query) {
      if (str.Length < query.Length) {
        // Can't match if the string is too short
        return false;
      }

      string queryLower = query.ToLower();
      string strLower = str.ToLower();

      int strIndex = 0;
      int queryIndex = 0;

      while (strIndex < strLower.Length) {
        if (strLower.Length - strIndex < queryLower.Length - queryIndex) {
          // Can't match if the remaining strings are too short
          break;
        }

        // Character match
        if (strLower[strIndex] == queryLower[queryIndex]) {

          // Word Boundary
          if (boundaryRegex.Match(str, strIndex, 1).Success) {
            queryIndex++;

            if (queryIndex > queryLower.Length - 1) {
              // We've reached the end of our query with successful matches
              return true;
            }
          }
        }

        strIndex++;
      }

      return false;
    }

    public static bool FuzzyMatch(string str, string query, out List<int> indices, out float score) {
      string queryLower = query.ToLower();
      string strLower = str.ToLower();

      indices = new List<int>();
      score = 0;

      if (strLower.Length < queryLower.Length) {
        // Can't match if the string is too short
        return false;
      }

      int strIndex = 0;
      int queryIndex = 0;
      int gap = 0;

      while (strIndex < strLower.Length) {
        if (strLower.Length - strIndex < queryLower.Length - queryIndex) {
          // Can't match if the remaining strings are too short
          break;
        }

        bool matchedChar = false;

        if (strLower[strIndex] == queryLower[queryIndex]) {

          // Word Boundary
          if (boundaryRegex.Match(str, strIndex, 1).Success) {
            score += 2;
            matchedChar = true;

          // Try to match this query char on a boundary in the future,
          // otherwise we fall back to a sequential character match.
          } else if (!BoundaryMatch(str.Substring(strIndex + 1), query.Substring(queryIndex))) {
            score += (1 / (gap + 1));
            matchedChar = true;
          }
        }

        if (matchedChar) {
          indices.Add(strIndex);

          queryIndex++;

          if (queryIndex > queryLower.Length - 1) {
            // If we have an exact match
            if (strLower == queryLower) {
              // Bump the score by an extra point for each char
              score += queryLower.Length;
            }

            // We've reached the end of our query with successful matches
            return true;
          }

          gap = 0;
        } else {
          gap++;
        }

        strIndex++;
      }

      return false;
    }
  }
}
