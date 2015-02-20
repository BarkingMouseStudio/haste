using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public static class HasteStringUtils {

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

    public static string GetBoundaries(this String str) {
      int len = str.Length;
      StringBuilder matches = new StringBuilder();

      char c, _c;
      for (int i = 0; i < len; i++) {
        c = str[i];

        // Is it a word char at the beginning of the string?
        if (i == 0 && !char.IsPunctuation(c)) {
          matches.Append(c);
          continue;
        }

        if (i > 0) {
          _c = str[i - 1];

          // Include extensions
          if (c == '.') {
            matches.Append(c);
            continue;
          }

          // Is it an upper char following a non-upper char?
          if (char.IsUpper(c) && char.IsLetter(_c) && !char.IsUpper(_c)) {
            matches.Append(c);
            continue;
          }

          // Is it a post-boundary word char
          if (char.IsLetter(c) && char.IsPunctuation(_c)) {
            matches.Append(c);
            continue;
          }
        }
      }

      return matches.ToString();
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