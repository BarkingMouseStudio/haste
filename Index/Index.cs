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

    Result[] Filter(string query);
  }

  public abstract class AbstractIndex : Index {

    protected HashSet<Item> index;

    public AbstractIndex() {
      index = new HashSet<Item>();
    }

    public virtual void Rebuild() {}

    public Result[] Filter(string query) {
      if (query.Length == 0) {
        return new Result[0];
      }

      query = query.ToLower();
      IList<Result> matches = new List<Result>();

      foreach (Item item in index) {
        int pathIndex = 0;
        int queryIndex = 0;
        int score = 0;

        string path = item.Path.ToLower();

        while (pathIndex < path.Length) {
          if (path[pathIndex] == query[queryIndex]) {
            queryIndex++;
            score++;

            if (queryIndex >= query.Length) {
              matches.Add(new Result(item, score));
              break;
            }
          }

          pathIndex++;
        }

        Logger.Info("Score", score, item.Path);
      }

      return matches.OrderBy(r => r.Score).ToArray();
    }
  }
}
