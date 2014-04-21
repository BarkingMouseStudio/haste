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
        AddSource(source);
      }

      Rebuild();
    }

    public void AddSource(Source source) {
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
      // TODO: Sorted Dictionary => Concat List
      IEnumerable<Result> results = new List<Result>();
      foreach (KeyValuePair<Source, Index> index in indices) {
        var res = index.Value.Filter(query, count);
        results = results.Concat(res);
      }
      return results.ToArray();
    }
  }
}
