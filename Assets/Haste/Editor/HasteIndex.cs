using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteIndex {

    IDictionary<char, HashSet<HasteItem>> index = new Dictionary<char, HashSet<HasteItem>>();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the indexing including each indexed reference
    public int Size { get; protected set; }

    public static char[] GetBoundaryChars(string path) {
      int len = path.Length;
      List<char> matches = new List<char>();

      char c, _c;
      for (int i = 0; i < len; i++) {
        c = path[i];

        // Is it a word char at the beginning of the string?
        if (i == 0 && !Char.IsPunctuation(c)) {
          matches.Add(c);
          continue;
        }

        if (i > 0) {
          _c = path[i - 1];

          // Include extensions
          if (c == '.') {
            matches.Add(c);
            continue;
          }

          // Is it an upper char following a non-upper char?
          if (Char.IsUpper(c) && Char.IsLetter(_c) && !Char.IsUpper(_c)) {
            matches.Add(c);
            continue;
          }

          // Is it a post-boundary word char
          if (Char.IsLetter(c) && Char.IsPunctuation(_c)) {
            matches.Add(c);
            continue;
          }
        }
      }

      return matches.ToArray();
    }

    public void Add(HasteItem item) {
      // System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
      Count++;

      char c_;
      foreach (char c in GetBoundaryChars(item.Path)) {
        c_ = Char.ToLower(c);

        if (!index.ContainsKey(c_)) {
          index.Add(c_, new HashSet<HasteItem>());
        }

        index[c_].Add(item);
        Size++;
      }

      // timer.Stop();
      // HasteDebug.Info("Ticks for {1}: {0}", timer.ElapsedTicks, item.Path);
    }

    public void Remove(HasteItem item) {
      Count--;

      char c_;
      foreach (char c in GetBoundaryChars(item.Path)) {
        c_ = Char.ToLower(c);

        if (index.ContainsKey(c_)) {
          index[c_].Remove(item);
          Size--;
        }
      }
    }

    public void Clear() {
      index.Clear();
      Count = 0;
      Size = 0;
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

    public IHasteResult[] Filter(string query, int resultCount) {
      if (query.Length == 0) {
        return new IHasteResult[0];
      }

      char c = Char.ToLower(query[0]);
      if (!index.ContainsKey(c)) {
        return new IHasteResult[0];
      }

      int queryBits = LetterBitsetFromString(query);

      // Filter
      var matches = index[c].Where(m => {
        return m.ContainsChars(queryBits) && m.IsSubsequence(query);
      });

      // Score
      var results = matches.Select(m => {
        // float score = m.IndexSum(query);
        List<int> indices = new List<int>();
        return Haste.Types.GetType(m, 0, indices);
      });

      // Sort then take, otherwise we loose good results
      return results.OrderByDescending(r => r, new HasteResultComparer())
        .Take(resultCount)
        .ToArray();
    }
  }
}
