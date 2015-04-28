using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
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

    public HasteIndex() {
      LastResults = emptyResults;
    }

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

    public bool IsFiltering { get; private set; }

    public IHasteResult[] LastResults { get; private set; }

    public IHasteResult[] FilterSync(string query, int resultCount) {
      var filtering = Filter(query, resultCount);
      while (filtering.MoveNext()) {
        continue;
      }
      return LastResults;
    }

    // TODO: Consider moving filtering + results out of index class
    public IEnumerator Filter(string query, int resultCount) {
      if (IsFiltering) {
        yield break;
      }

      IsFiltering = true;

      int queryLen = query.Length;
      if (queryLen == 0) {
        LastResults = emptyResults;
        yield break;
      }

      string queryLower = query.ToLowerInvariant();

      // Lookup bucket by first char
      HashSet<IHasteItem> bucket;
      if (!index.TryGetValue(queryLower[0], out bucket)) {
        LastResults = emptyResults;
        yield break;
      }

      int queryBits = HasteStringUtils.LetterBitsetFromString(queryLower);

      // Perform fast subsequence filtering
      var matches = new List<IHasteItem>();
      char q = queryLower[0];

      foreach (var m in bucket) {
        if (m.PathLower.Length < queryLen) {
          continue;
        }

        bool firstCharName = m.NameLower.Length > 0 && m.NameLower[0] == q;
        bool firstCharPath = m.PathLower.Length > 0 && m.PathLower[0] == q;
        bool firstCharExtension = m.ExtensionLower.Length > 0 && m.ExtensionLower[0] == q;
        if (!firstCharExtension && !firstCharName && !firstCharPath) {
          // TODO: Move to bucketing instead of boundaries
          continue;
        }

        var contains = HasteStringUtils.ContainsChars(m.Bitset, queryBits);
        if (!contains) {
          continue;
        }

        var subsequence = HasteStringUtils.ContainsSubsequence(m.PathLower, queryLower, m.PathLower.Length, queryLen);
        if (!subsequence) {
          continue;
        }

        matches.Add(m);
        yield return null;
      }

      // Score, sort then take (otherwise we loose good results)
      // TODO: Map, sort, take loops
      var results = new List<IHasteResult>(matches.Count);
      foreach (var m in matches) {
        results.Add(m.GetResult(queryLower, queryLen));
        yield return null;
      }

      LastResults = results
        .OrderBy(r => r, comparer)
        .Take(resultCount)
        .ToArray();
      IsFiltering = false;
    }
  }
}
