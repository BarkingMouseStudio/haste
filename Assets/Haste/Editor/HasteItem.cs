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
  }
}