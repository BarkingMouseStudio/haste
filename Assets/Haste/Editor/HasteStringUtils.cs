using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public static class HasteStringUtils {

    public static int LongestCommonSubsequenceLength(string first, string second) {
      string longer  = first.Length > second.Length ? first : second;
      string shorter = first.Length > second.Length ? second : first;

      int longerLen  = longer.Length;
      int shorterLen = shorter.Length;

      int[] previous = new int[shorterLen + 1];
      int[] current = new int[shorterLen + 1];

      for (int i = 0; i < longerLen; i++) {
        for (int j = 0; j < shorterLen; j++) {
          if (longer[i] == shorter[j]) {
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

    public static int LetterBitsetFromString(string str) {
      int bits = 0;
      int mask;
      foreach (char c in str) {
        mask = 1 << (int)c;
        bits |= mask;
      }
      return bits;
    }

    public static bool ContainsChars(int a, int b) {
      return (a & b) == b;
    }

    public static bool ContainsSubsequence(this String str, string query) {
      var queryLen = query.Length;
      var strLen = str.Length;

      if (queryLen > strLen) {
        return false;
      }

      char strChar, queryChar;
      int queryIndex = 0;
      int strIndex = 0;

      while (strIndex < strLen && queryIndex < queryLen) {
        queryChar = query[queryIndex];
        strChar = str[strIndex];

        if (queryChar == strChar) {
          queryIndex++;
          strIndex++;
        } else {
          strIndex++;
        }
      }

      return queryIndex == queryLen;
    }

    public static bool GetMatchIndices(string pathLower, string queryLower, int offset, int[] boundaryIndices, out int[] indices) {
      List<int> indices_ = new List<int>();
      float score = 0;

      int pathLen = pathLower.Length;
      int queryLen = queryLower.Length;

      // Can't match if the string is too short
      if (pathLen < queryLen) {
        indices = indices_.ToArray();
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
          indices_.Add(pathIndex + offset);
          queryIndex++;
          gap = 0;

          if (queryIndex > queryLower.Length - 1) {
            // If we have an exact match
            if (pathLower == queryLower) {
              // Bump the score by an extra point for each char
              score += queryLower.Length;
            }

            // We've reached the end of our query with successful matches
            indices = indices_.ToArray();
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

      indices = indices_.ToArray();
      return false;
    }

    public static int[] GetBoundaryIndices(string str) {
      int len = str.Length;
      List<int> indices = new List<int>();

      char c, _c;
      for (int i = 0; i < len; i++) {
        c = str[i];

        // Is it a word char at the beginning of the string?
        if (i == 0) {
          if (!char.IsPunctuation(c)) {
            indices.Add(i);
          }
        } else {
          _c = str[i - 1];

          // Include extensions
          if (c == '.') {
            indices.Add(i);
            continue;
          }

          // Is it an upper char proceeding a lowercase char or whitespace?
          if (char.IsUpper(c) && !char.IsUpper(_c)) {
            indices.Add(i);
            continue;
          }

          // Is it a post-boundary word char?
          if (char.IsLetter(c) && char.IsPunctuation(_c)) {
            indices.Add(i);
            continue;
          }
        }
      }

      return indices.ToArray();
    }

    public static string GetBoundaries(string str) {
      int len = str.Length;
      StringBuilder matches = new StringBuilder();

      char c, _c;
      for (int i = 0; i < len; i++) {
        c = str[i];

        // Is it a word char at the beginning of the string?
        if (i == 0) {
          if (!char.IsPunctuation(c)) {
            matches.Append(c);
          }
        } else {
          _c = str[i - 1];

          // Include extensions
          if (c == '.') {
            matches.Append(c);
            continue;
          }

          // Is it an upper char proceeding a lowercase char or whitespace?
          if (char.IsUpper(c) && !char.IsUpper(_c)) {
            matches.Append(c);
            continue;
          }

          // Is it a post-boundary word char?
          if (char.IsLetter(c) && char.IsPunctuation(_c)) {
            matches.Append(c);
            continue;
          }
        }
      }

      return matches.ToString();
    }

    public static string BoldLabel(string str, int[] indices, string boldStart = "<color=\"white\">", string boldEnd = "</color>") {
      if (indices == null || indices.Length == 0) {
        return str;
      }

      StringBuilder bolded = new StringBuilder();
      int j = 0;
      for (int i = 0; i < str.Length; i++) {
        if (j < indices.Length && i == indices[j]) {
          bolded.Append(boldStart).Append(str[i]).Append(boldEnd);
          j++;
        } else {
          bolded.Append(str[i]);
        }
      }

      // // Faster; may not work correctly?
      // StringBuilder bolded = new StringBuilder(str);
      // int index;
      // int offset = 0;
      // for (int i = 0; i < indices.Length; i++) {
      //   index = indices[i];
      //   bolded.Insert(index + offset, boldStart);
      //   offset += boldStart.Length;
      //   bolded.Insert(index + offset + 1, boldEnd);
      //   offset += boldEnd.Length + 1;
      // }
      return bolded.ToString();
    }

    public static string TrimStart(this String str, string prefix) {
      if (str.StartsWith(prefix)) {
        str = str.Remove(0, prefix.Length);
      }
      return str;
    }

    public static string TrimEnd(this String str, string postfix) {
      if (str.EndsWith(postfix)) {
        str = str.Remove(str.Length - postfix.Length, postfix.Length);
      }
      return str;
    }

    public static string[] Split(this String str, int[] at) {
      List<string> parts = new List<string>(at.Length + 1);

      int offset = 0;
      foreach (var index in at) {
        int offsetIndex = index - offset;
        if (offsetIndex == 0) {
          continue;
        }

        string part = str.Substring(0, offsetIndex);

        if (part != "") {
          parts.Add(part);
        }

        str = str.Substring(offsetIndex);
        offset += offsetIndex;
      }

      if (str != "") {
        parts.Add(str);
      }

      return parts.ToArray();
    }
  }
}