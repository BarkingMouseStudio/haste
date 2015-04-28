using UnityEngine;
using UnityEditor;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Haste {

  public static class HasteStringUtils {

    public static int LongestCommonSubsequenceLength(string first, string second) {
      string longer = first.Length > second.Length ? first : second;
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
            if (current[j] >= previous[j + 1]) {
              current[j + 1] = current[j];
            } else {
              current[j + 1] = previous[j + 1];
            }
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
      char c;
      for (int i = 0; i < str.Length; i++) {
        c = str[i];
        mask = 1 << (int)c;
        bits |= mask;
      }
      return bits;
    }

    public static bool ContainsChars(int a, int b) {
      return (a & b) == b;
    }

    public static bool ContainsSubsequence(string str, string query, int strLen, int queryLen) {
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

    public static List<List<int>> GetQueryMatchIndices(string path, string query, int[] boundaryIndices) {
      List<List<int>> queryMatchIndices = new List<List<int>>();

      char c;
      for (int queryIndex = 0; queryIndex < query.Length; queryIndex++) {
        c = query[queryIndex];

        List<int> orderedChars = new List<int>(path.Length);
        List<int> nonBoundaryChars = new List<int>();

        for (int pathIndex = 0; pathIndex < path.Length; pathIndex++) {
          if (c == path[pathIndex]) {
            if (Array.IndexOf(boundaryIndices, pathIndex) != -1) {
              orderedChars.Add(pathIndex);
            } else {
              nonBoundaryChars.Add(pathIndex);
            }
          }
        }

        orderedChars.AddRange(nonBoundaryChars);
        queryMatchIndices.Add(orderedChars);
      }

      return queryMatchIndices;
    }

    public static int[] GetWeightedSubsequence(string path, string query, int[] boundaryIndices) {
      Stack<int> results = new Stack<int>(query.Length);

      List<List<int>> queryIndices = GetQueryMatchIndices(path, query, boundaryIndices);

      int invalidResult = -1;

      int i = 0;
      while (results.Count < query.Length) {
        List<int> charIndices = queryIndices[i];

        if (invalidResult != -1) {
          queryIndices[i] = charIndices = charIndices.Where(x => x < invalidResult).ToList();
        }

        bool matchedSomething = false;
        int greatestResult = -1;
        for (int j = 0; j < charIndices.Count; j++) {
          if (charIndices[j] > greatestResult) {
            greatestResult = charIndices[j];
            if (results.Count == 0 || greatestResult > results.Peek()) {
              matchedSomething = true;
              break;
            }
          }
        }

        if (matchedSomething) {
          results.Push(greatestResult);
          i++;
          invalidResult = -1;
        } else {
          results.Pop();
          i--;
          invalidResult = greatestResult;
        }
      }

      return results.Reverse().ToArray();
    }

    public static string GetFileName(string path) {
      var len = path.Length;
      if (len == 0) {
        return "";
      }

      var sep = path.LastIndexOf('/');
      if (sep == len - 1) {
        path = path.TrimEnd(new []{'/'});
        sep = path.LastIndexOf('/');
      }

      if (sep != -1) {
        return path.Substring(sep + 1);
      } else {
        return path;
      }
    }

    public static string GetExtension(string path) {
      var len = path.Length;
      if (len == 0) {
        return "";
      }

      var sep = path.LastIndexOf('/');

      // Remove trailing slash before getting filename
      if (sep == len - 1) {
        path = path.TrimEnd(new []{'/'});
        sep = path.LastIndexOf('/');
      }

      int ext = -1;
      if (sep != -1) {
        ext = path.IndexOf('.', sep);
      } else {
        ext = path.LastIndexOf('.');
      }

      if (ext != -1) {
        return path.Substring(ext + 1);
      } else {
        return "";
      }
    }

    public static string GetFileNameWithoutExtension(string path) {
      var len = path.Length;
      if (len == 0) {
        return "";
      }

      var sep = path.LastIndexOf('/');

      // Remove trailing slash before getting filename
      if (sep == len - 1) {
        path = path.TrimEnd(new []{'/'});
        sep = path.LastIndexOf('/');
      }

      var ext = path.LastIndexOf('.');
      if (sep != -1 && ext != -1) {
        if (ext < sep) {
          sep = sep + 1;
          return path.Substring(sep);
        } else {
          sep = sep + 1;
          return path.Substring(sep, ext - sep);
        }
      } else if (sep != -1) {
        sep = sep + 1;
        return path.Substring(sep);
      } else if (ext != -1) {
        return path.Substring(0, ext);
      } else {
        return path;
      }
    }

    public static int[] GetBoundaryIndices(string str) {
      int len = str.Length;
      List<int> indices = new List<int>();

      if (len == 0) {
        return indices.ToArray();
      }

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
          if (char.IsLetterOrDigit(c) && (char.IsPunctuation(_c) || _c == ' ')) {
            indices.Add(i);
            continue;
          }
        }
      }

      return indices.ToArray();
    }

    // It's faster to lowercase each char during iteration rather
    // than ToLowerInvariant at the end.
    public static string GetBoundaries(string str) {
      int len = str.Length;
      if (len == 0) {
        return "";
      }

      // Initializing the string builder with some default capacity helps.
      StringBuilder matches = new StringBuilder(len / 2, len);

      char c, _c;
      for (int i = 0; i < len; i++) {
        c = str[i];

        // Is it a word char at the beginning of the string?
        if (i == 0) {
          if (!char.IsPunctuation(c)) {
            matches.Append(char.ToLowerInvariant(c));
          }
        } else {
          _c = str[i - 1];

          // Include extensions
          if (c == '.') {
            matches.Append(char.ToLowerInvariant(c));
            continue;
          }

          // Is it an upper char proceeding a lowercase char or whitespace?
          if (char.IsUpper(c) && !char.IsUpper(_c)) {
            matches.Append(char.ToLowerInvariant(c));
            continue;
          }

          // Is it a post-boundary word char?
          if (char.IsLetterOrDigit(c) && (char.IsPunctuation(_c) || _c == ' ')) {
            matches.Append(char.ToLowerInvariant(c));
            continue;
          }
        }
      }

      return matches.ToString();
    }

    public static string BoldLabel(string str, int[] indices, string boldStart = "<color=\"white\">", string boldEnd = "</color>") {
      int indicesLen = indices.Length;
      if (indicesLen == 0) {
        return str;
      }

      // Initialize StringBuilder with maximum new length.
      int maxCap = str.Length + ((boldStart.Length + boldEnd.Length) * indicesLen);
      StringBuilder bolded = new StringBuilder(str, maxCap);

      int index;
      int offset = 0;

      for (int i = 0; i < indicesLen; i++) {
        index = indices[i];
        bolded.Insert(index + offset, boldStart);
        offset += boldStart.Length;
        bolded.Insert(index + offset + 1, boldEnd);
        offset += boldEnd.Length;
      }

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
