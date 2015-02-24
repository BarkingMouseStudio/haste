using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteIndex {

    private static readonly IHasteResult[] emptyResults = new IHasteResult[0];

    IDictionary<char, HashSet<HasteItem>> index =
      new Dictionary<char, HashSet<HasteItem>>();

    HasteResultComparer comparer = new HasteResultComparer();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the index including each indexed reference
    public int Size { get; protected set; }

    public void Add(HasteItem item) {
      Count++;

      foreach (char c in item.BoundariesLower) {
        if (!index.ContainsKey(c)) {
          index.Add(c, new HashSet<HasteItem>());
        }

        index[c].Add(item);
        Size++;
      }
    }

    public void Remove(HasteItem item) {
      Count--;

      foreach (char c in item.BoundariesLower) {
        if (index.ContainsKey(c)) {
          index[c].Remove(item);
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
        return emptyResults;
      }

      string queryLower = query.ToLower();

      // Lookup bucket by first char
      HashSet<HasteItem> bucket;
      if (!index.TryGetValue(queryLower[0], out bucket)) {
        return emptyResults;
      }

      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);

      // Perform fast subsequence filtering
      var matches = bucket.Where(m => {
        if (m.PathLower.Length < queryLower.Length) {
          return false;
        }

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

      // Score, sort then take (otherwise we loose good results)
      return matches.Select(m => Haste.Types.GetType(m, queryLower))
        .OrderBy(r => r, comparer)
        .Take(resultCount)
        .ToArray();
    }
  }
}
