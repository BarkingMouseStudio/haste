using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class IndexManager : Index {

    private IDictionary<Source, Index> indices;

    public IndexManager(params Source[] sources) {
      indices = new Dictionary<Source, Index>();

      foreach (var source in sources) {
        switch (source) {
          case Source.Hierarchy:
            indices.Add(source, new HierarchyIndex());
            break;
          case Source.Project:
            indices.Add(source, new ProjectIndex());
            break;
          case Source.Editor:
            indices.Add(source, new EditorIndex());
            break;
        } 
      }
    }

    public void RemoveSource(Source source) {
      indices.Remove(source);
    }

    public void Rebuild(Source source) {
      if (indices.ContainsKey(source)) {
        indices[source].Rebuild();
      }
    }

    public void Rebuild() {
      foreach (var index in indices.Values) {
        index.Rebuild();
      }
    }

    public Result[] Filter(string query, int count) {
      // Filter each index building a list of results for each
      List<Result[]> results = new List<Result[]>();
      foreach (Index index in indices.Values) {
        results.Add(index.Filter(query, count));
      }

      // Sort the list of results by each sub-lists top score
      IOrderedEnumerable<Result[]> sortedResults = results.OrderBy(x => {
        return x.Length > 0 ? -x[0].Score : -1;
      });

      // Concat each sub-list
      IEnumerable<Result> finalResults = new List<Result>();
      foreach (var result in sortedResults) {
        finalResults = finalResults.Concat(result);
      }
      return finalResults.ToArray();
    }
  }
}
