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

    public HasteResult[] Filter(string query, int countPerGroup) {
      if (query.Length == 0) {
        return new HasteResult[0];
      }

      char c = Char.ToLower(query[0]);

      if (!index.ContainsKey(c)) {
        return new HasteResult[0];
      }

      HasteMatcher matcher = new HasteMatcher(query);

      IList<HasteResult> matches = new List<HasteResult>();
      foreach (HasteItem item in index[c]) {
        float score;

        string filename = Path.GetFileNameWithoutExtension(item.Path);
        if (matcher.Match(filename, 2, out score)) { // Filename
          matches.Add(new HasteResult(item, score));
        } else if (matcher.Match(item.Path, 1, out score)) { // Full path
          matches.Add(new HasteResult(item, score));
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
