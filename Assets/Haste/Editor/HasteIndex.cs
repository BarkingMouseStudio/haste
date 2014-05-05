using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteIndex {

    readonly Regex boundaryRegex = new Regex(@"\b\w");

    IDictionary<char, HashSet<HasteItem>> index = new Dictionary<char, HashSet<HasteItem>>();

    public void Add(string path, HasteSource source) {
      MatchCollection matches = boundaryRegex.Matches(path);

      HasteItem item = new HasteItem(path, source);
      foreach (Match match in matches) {
        char c = Char.ToLower(match.Value[0]);

        if (!index.ContainsKey(c)) {
          index.Add(c, new HashSet<HasteItem>());
        }

        index[c].Add(item);
      }
    }

    public void Remove(string path, HasteSource source) {
      MatchCollection matches = boundaryRegex.Matches(path);

      HasteItem item = new HasteItem(path, source);
      foreach (Match match in matches) {
        char c = Char.ToLower(match.Value[0]);

        if (index.ContainsKey(c)) {
          index[c].Remove(item);
        }
      }
    }

    public void Clear() {
      index = new Dictionary<char, HashSet<HasteItem>>();
    }

    public HasteResult[] Filter(string query, int countPerGroup) {
      if (query.Length == 0) {
        return new HasteResult[0];
      }

      char c = Char.ToLower(query[0]);

      if (!index.ContainsKey(c)) {
        return new HasteResult[0];
      }

      IList<HasteResult> matches = new List<HasteResult>();
      foreach (HasteItem item in index[c]) {
        float score;

        string filename = Path.GetFileNameWithoutExtension(item.Path);
        string directory = Path.GetDirectoryName(item.Path);
        IList<int> indices;

        if (HasteFuzzyMatching.FuzzyMatch(filename, query, out indices, out score)) { // Filename
          // Increment indices to account for being only the filename
          if (directory.Length > 0) {
            for (int i = 0; i < indices.Count; i++) {
              indices[i] += directory.Length + 1; // Add 1 to account for slash
            }
          }

          matches.Add(new HasteResult(item, indices, score * 2));
          continue;
        }

        if (HasteFuzzyMatching.FuzzyMatch(item.Path, query, out indices, out score)) { // Full path
          matches.Add(new HasteResult(item, indices, score));
          continue;
        }
      }

      return matches
        .GroupBy(r => r.Source) // Group by source
        .Select(g => {
          // Order each group by score and take the top N
          return g
            .OrderByDescending(r => r.Score)
            .ThenBy(r => Path.GetFileNameWithoutExtension(r.Path))
            .Take(countPerGroup);
        })
        .OrderByDescending(g => g.First().Score) // Sort each group by score
        .SelectMany(g => g) // Flatten the groups
        .ToArray();
    }
  }
}
