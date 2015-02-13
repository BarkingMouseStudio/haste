using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteIndex {

    readonly Regex boundaryRegex = new Regex(@"\b(?<char>\w)|\w(?<char>[A-Z])|\.(?<char>\w)");

    IDictionary<char, HashSet<HasteItem>> index = new Dictionary<char, HashSet<HasteItem>>();

    // The number of unique items in the index
    public int Count { get; protected set; }

    // The total size of the indexing including each indexed reference
    public int Size { get; protected set; }

    public void Add(HasteItem item) {
      MatchCollection matches = boundaryRegex.Matches(item.Path);
      Count++;

      foreach (Match match in matches) {
        char c = Char.ToLower(match.Groups["char"].Value[0]);

        if (!index.ContainsKey(c)) {
          index.Add(c, new HashSet<HasteItem>());
        }

        index[c].Add(item);
        Size++;
      }
    }

    public void Remove(HasteItem item) {
      MatchCollection matches = boundaryRegex.Matches(item.Path);
      Count--;

      foreach (Match match in matches) {
        char c = Char.ToLower(match.Groups["char"].Value[0]);

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
        return new IHasteResult[0];
      }

      char c = Char.ToLower(query[0]);

      if (!index.ContainsKey(c)) {
        return new IHasteResult[0];
      }

      IList<IHasteResult> matches = new List<IHasteResult>();
      foreach (HasteItem item in index[c]) {
        List<int> indices;
        float score;

        string name = Path.GetFileNameWithoutExtension(item.Path);
        string path = Path.GetDirectoryName(item.Path); // Path excluding name

        // Try to match just the name
        if (HasteFuzzyMatching.FuzzyMatch(name, query, out indices, out score)) {
          // Increment indices to account for being only the name
          if (path.Length > 0) {
            for (int i = 0; i < indices.Count; i++) {
              indices[i] += path.Length + 1; // Add 1 to account for slash
            }
          }

          matches.Add(Haste.Types.GetType(item, score * 2, indices));
          continue;
        }

        // Then try to match the whole path
        if (HasteFuzzyMatching.FuzzyMatch(item.Path, query, out indices, out score)) {
          matches.Add(Haste.Types.GetType(item, score, indices));
          continue;
        }
      }

      // Sort then take, otherwise we loose good results
      return matches.OrderByDescending(r => {
          #if !IS_HASTE_PRO
          // Force menu item matches to the bottom in free version
          if (r.Item.Source == HasteMenuItemSource.NAME) {
            return 0;
          }
          #endif

          return r.Score;
        })
        .ThenBy(r => Path.GetFileNameWithoutExtension(r.Item.Path))
        .Take(resultCount)
        .ToArray();
    }
  }
}
