using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Haste {

  public interface Index {

    void Rebuild();

    Result[] Filter(string query, int count);
  }

  public abstract class AbstractIndex : Index {

    protected IDictionary<char, HashSet<Item>> index = new Dictionary<char, HashSet<Item>>();
    protected Regex boundaryRegex = new Regex(@"\b\w");

    public virtual void Rebuild() {}

    protected void AddItem(Item item) {
      MatchCollection matches = boundaryRegex.Matches(item.Path);

      foreach (Match match in matches) {
        char first = Char.ToLower(match.Value[0]);

        if (!index.ContainsKey(first)) {
          index.Add(first, new HashSet<Item>());
        }

        index[first].Add(item);
      }
    }

    protected void RemoveItem(string path) {
      MatchCollection matches = boundaryRegex.Matches(path);

      foreach (Match match in matches) {
        char first = Char.ToLower(match.Value[0]);

        if (index.ContainsKey(first)) {
          index[first].RemoveWhere(item => item.Path == path);
        }
      }
    }

    protected bool Match(string path, string query, int multiplier, out int score) {
      query = query.ToLower();
      string pathLower = path.ToLower();

      score = 0;

      if (pathLower.Length < query.Length) {
        // Can't match if the string is too short
        return false;
      }

      int pathIndex = 0;
      int queryIndex = 0;
      int gap = 0;

      while (pathIndex < pathLower.Length) {
        if (pathLower.Length - pathIndex < query.Length - queryIndex) {
          // Can't match if the remaining strings are too short
          return false;
        }

        if (pathLower[pathIndex] == query[queryIndex]) {
          queryIndex++;

          if (gap == 0) {
            score += multiplier;
          } else {
            Match boundaryMatch = boundaryRegex.Match(pathLower, pathIndex, 1);
            if (path[pathIndex] == Char.ToUpper(path[pathIndex]) || boundaryMatch.Success) {
              score += multiplier;
            }
          }

          if (queryIndex >= query.Length) {
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

    public Result[] Filter(string query, int count) {
      if (query.Length == 0) {
        return new Result[0];
      }

      char first = Char.ToLower(query[0]);

      if (!index.ContainsKey(first)) {
        return new Result[0];
      }

      IList<Result> matches = new List<Result>();
      foreach (Item item in index[first]) {
        int score;
        if (Match(item.Name, query, 2, out score)) {
          matches.Add(new Result(item, score));
        } else if (Match(item.Path, query, 1, out score)) {
          matches.Add(new Result(item, score));
        }
      }

      return matches.OrderBy(r => -r.Score).Take(count).ToArray();
    }
  }
}
