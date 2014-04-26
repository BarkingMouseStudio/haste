using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteIndex {

    protected readonly Regex boundaryRegex = new Regex(@"\b\w");

    protected IDictionary<char, HashSet<HasteItem>> index = new Dictionary<char, HashSet<HasteItem>>();

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

    protected bool Score(string path, string query, int multiplier, out int score) {
      string queryLower = query.ToLower();
      string pathLower = path.ToLower();

      score = 0;

      if (pathLower.Length < queryLower.Length) {
        // Can't match if the string is too short
        return false;
      }

      int pathIndex = 0;
      int queryIndex = 0;
      int gap = 0;

      while (pathIndex < pathLower.Length) {
        if (pathLower.Length - pathIndex < queryLower.Length - queryIndex) {
          // Can't match if the remaining strings are too short
          return false;
        }

        if (pathLower[pathIndex] == queryLower[queryIndex]) {

          // Word Boundary
          if (boundaryRegex.Match(pathLower, pathIndex, 1).Success) {
            score += 4 * multiplier;

          // Caps Boundary
          } else if (path[pathIndex] == Char.ToUpper(path[pathIndex])) {
            score += 3 * multiplier;

          // Sequential Char
          } else if (gap == 0) {
            score += 2 * multiplier;

          // Non-sequential Char
          } else {
            score += 1 * multiplier;
          }

          queryIndex++;

          if (queryIndex >= queryLower.Length) {
            // We've reached the end of our query with successful matches
            return true;
          }

          gap = 0;
        } else {
          gap++;
        }

        pathIndex++;
      }

      return false;
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
        int score;
        string path = item.Source == HasteSource.Project ?
          HasteUtils.GetRelativeAssetPath(item.Path) : item.Path;

        if (Score(Path.GetFileNameWithoutExtension(path), query, 2, out score)) { // Item Name
          matches.Add(new HasteResult(path, item.Source, score));
        } else if (Score(path, query, 1, out score)) { // Full Path
          matches.Add(new HasteResult(path, item.Source, score));
        }
      }

      return matches
        .GroupBy(r => r.Source) // Group by source
        .Select(g => {
          // Order each group by score and take the top N
          return g.OrderByDescending(r => r.Score).Take(countPerGroup);
        })
        .OrderByDescending(g => g.First().Score) // Sort each group by score
        .SelectMany(g => g) // Flatten the groups
        .ToArray();
    }
  }
}
