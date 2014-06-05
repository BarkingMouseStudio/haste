using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteItem {
    public string Path;
    public int Id;
    public string Source;

    public HasteItem(string path, int id, string source) {
      Path = path;
      Id = id;
      Source = source;
    }

    public override string ToString() {
      return System.String.Format("<{0}, {1}, {2}>", Path, Id, Source);
    }
  }
}