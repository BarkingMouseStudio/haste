using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public delegate IHasteResult HasteTypeFactory(HasteItem item, string query);

  // Factory manager for converting between sources and results.
  // TODO: Have HasteItem<T> provide result factory. Even better: since HasteItems shouldn't be structs (too big) generalize HasteItems and have them return their result directly for the concrete type. IHasteItem => HasteHierarchyItem => HasteHierarchyResult
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

    public IHasteResult GetType(HasteItem item, string query) {
      HasteTypeFactory factory;
      if (types.TryGetValue(item.Source, out factory)) {
        return factory(item, query);
      } else {
        return null;
      }
    }
  }
}
