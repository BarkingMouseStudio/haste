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

    protected HashSet<Item> index;
    protected Regex boundaryRegex = new Regex(@"\b\w");

    public AbstractIndex() {
      index = new HashSet<Item>();
    }

    public virtual void Rebuild() {}

    public bool Match(string path, string query, int multiplier, out int score) {
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

      IList<Result> matches = new List<Result>();
      foreach (Item item in index) {
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
