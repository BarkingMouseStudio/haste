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

    public static string GetBoundaries(string str, out int[] boundaryIndices) {
      int len = str.Length;
      StringBuilder matches = new StringBuilder();
      List<int> indices = new List<int>();

      char c, _c;
      for (int i = 0; i < len; i++) {
        c = str[i];

        // Is it a word char at the beginning of the string?
        if (i == 0) {
          if (!char.IsPunctuation(c)) {
            indices.Add(i);
            matches.Append(c);
          }
        } else {
          _c = str[i - 1];

          // Include extensions
          if (c == '.') {
            indices.Add(i);
            matches.Append(c);
            continue;
          }

          // Is it an upper char proceeding a lowercase char or whitespace?
          if (char.IsUpper(c) && !char.IsUpper(_c)) {
            indices.Add(i);
            matches.Append(c);
            continue;
          }

          // Is it a post-boundary word char?
          if (char.IsLetter(c) && char.IsPunctuation(_c)) {
            indices.Add(i);
            matches.Append(c);
            continue;
          }
        }
      }

      boundaryIndices = indices.ToArray();
      return matches.ToString();
    }

    public static string BoldLabel(string str, int[] indices, string boldStart = "<color=\"white\">", string boldEnd = "</color>") {
      if (indices.Length == 0) {
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