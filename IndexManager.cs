using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Haste {

  public class IndexManager {

    private IDictionary<Source, Index> indices;

    public IndexManager(params Source[] sources) {
      indices = new Dictionary<Source, Index>();

      foreach (var source in sources) {
        AddSource(source);
      }

      RebuildAll();
    }

    public void AddSource(Source source) {
      switch (source) {
        case Source.Hierarchy:
          indices.Add(source, new HierarchyIndex());
          break;
        case Source.Project:
          indices.Add(source, new ProjectIndex());
          break;
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

    public void RebuildAll() {
      foreach (var index in indices.Values) {
        index.Rebuild();
      }
    }

    public Result[] Filter(string query) {
      // TODO: Sorted Dictionary => Concat List
      IEnumerable<Result> results = new List<Result>();
      foreach (KeyValuePair<Source, Index> index in indices) {
        var res = index.Value.Filter(query);
        results = results.Concat(res);
      }
      return results.ToArray();
    }
  }
}
