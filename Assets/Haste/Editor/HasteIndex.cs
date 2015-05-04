using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class HasteIndex {

    private static readonly IHasteResult[] emptyResults = new IHasteResult[0];

    readonly IDictionary<char, HashSet<IHasteItem>> index =
      new Dictionary<char, HashSet<IHasteItem>>();

    readonly HasteResultComparer comparer = new HasteResultComparer();

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
      char q0 = queryLower[0];

      // Perform fast subsequence filtering
      var matches = bucket.Where(m => {
        if (m.PathLower.Length < queryLen) {
          return false;
        }

        bool firstCharName = m.NameLower.Length > 0 && m.NameLower[0] == q0;
        bool firstCharPath = m.PathLower.Length > 0 && m.PathLower[0] == q0;
        bool firstCharExtension = q0 == '.' || m.ExtensionLower.Length > 0 && m.ExtensionLower[0] == q0;
        if (!firstCharName && !firstCharPath && !firstCharExtension) {
          return false;
        }

        bool contains = HasteStringUtils.ContainsChars(m.Bitset, queryBits);
        if (!contains) {
          return false;
        }

        bool subsequence = HasteStringUtils.ContainsSubsequence(m.PathLower, queryLower, m.PathLower.Length, queryLen);
        if (!subsequence) {
          return false;
        }

        return true;
      });

      // Score, sort then take (otherwise we loose good results)
      return matches.Select(m => {
          var r = m.GetResult(queryLower, queryLen);
          r.Score = HasteScoring.Score(r);
          return r;
        })
        .OrderBy(r => r, comparer)
        .Take(resultCount)
        .ToArray();
    }
  }
}
