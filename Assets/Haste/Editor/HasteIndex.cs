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

    IDictionary<char, HashSet<IHasteItem>> index =
      new Dictionary<char, HashSet<IHasteItem>>();

    HasteResultComparer comparer = new HasteResultComparer();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the index including each indexed reference
    public int Size { get; protected set; }

    public void Add(IHasteItem item) {
      Count++;

      foreach (char c in item.BoundariesLower) {
        if (!index.ContainsKey(c)) {
          index.Add(c, new HashSet<IHasteItem>());
        }

        index[c].Add(item);
        Size++;
      }
    }

    public void Remove(IHasteItem item) {
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
      int queryLen = query.Length;
      if (queryLen == 0) {
        return emptyResults;
      }

      string queryLower = query.ToLowerInvariant();

      // Lookup bucket by first char
      HashSet<IHasteItem> bucket;
      if (!index.TryGetValue(queryLower[0], out bucket)) {
        return emptyResults;
      }

      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);
      // TODO: Require first char of name or path to match query?

      // Perform fast subsequence filtering
      var matches = bucket.Where(m => {
        if (m.PathLower.Length < queryLen) {
          return false;
        }

        var contains = HasteStringUtils.ContainsChars(m.Bitset, queryBits);
        if (!contains) {
          return false;
        }

        var subsequence = HasteStringUtils.ContainsSubsequence(m.PathLower, queryLower, m.PathLower.Length, queryLen);
        if (!subsequence) {
          return false;
        }

        return true;
      });

      // Score, sort then take (otherwise we loose good results)
      return matches.Select(m => m.GetResult(queryLower, queryLen))
        .OrderBy(r => r, comparer)
        .Take(resultCount)
        .ToArray();
    }
  }
}
