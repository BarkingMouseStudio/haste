using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteIndex {

    IDictionary<char, HashSet<HasteItem>> index =
      new Dictionary<char, HashSet<HasteItem>>();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the index including each indexed reference
    public int Size { get; protected set; }

    public void Add(HasteItem item) {
      Count++;

      char c_;
      foreach (char c in item.Boundaries) {
        c_ = char.ToLower(c);

        if (!index.ContainsKey(c_)) {
          index.Add(c_, new HashSet<HasteItem>());
        }

        index[c_].Add(item);
        Size++;
      }
    }

    public void Remove(HasteItem item) {
      Count--;

      char c_;
      foreach (char c in item.Boundaries) {
        c_ = char.ToLower(c);

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

    public IHasteResult[] Filter(string query, int resultCount) {
      if (query.Length == 0) {
        return new IHasteResult[0];
      }

      char c = char.ToLower(query[0]);
      HashSet<HasteItem> bucket;
      if (!index.TryGetValue(c, out bucket)) {
        return new IHasteResult[0];
      }

      string queryLower = query.ToLower();
      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);

      // Filter
      var matches = bucket.Where(m => {
        var contains = HasteStringUtils.ContainsChars(m.Bitset, queryBits);
        if (!contains) {
          return false;
        }

        var subsequence = m.PathLower.ContainsSubsequence(queryLower);
        if (!subsequence) {
          return false;
        }

        return true;
      });

      // Score
      var results = matches.Select(m => {
        return Haste.Types.GetType(m, query);
      });

      // Sort then take (otherwise we loose good results)
      var comparer = new HasteResultComparer();
      return results.OrderByDescending(r => r, comparer)
        .Take(resultCount)
        .ToArray();
    }
  }
}
