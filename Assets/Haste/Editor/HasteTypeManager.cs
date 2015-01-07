using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate IHasteResult HasteTypeFactory(HasteItem item, float score, List<int> indices);

  // Factory manager for converting between sources and results.
  public class HasteTypeManager {

    static IDictionary<string, HasteTypeFactory> types =
      new Dictionary<string, HasteTypeFactory>();

    public void AddType(string name, HasteTypeFactory factory) {
      if (!types.ContainsKey(name)) {
        types.Add(name, factory);
      }
    }

    public void RemoveType(string name) {
      types.Remove(name);
    }

    public IHasteResult GetType(HasteItem item, float score, List<int> indices) {
      HasteTypeFactory factory;
      if (types.TryGetValue(item.Source, out factory)) {
        return factory(item, score, indices);
      } else {
        return null;
      }
    }
  }
}
