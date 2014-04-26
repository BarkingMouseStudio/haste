using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteItem {
    public string Path;
    public HasteSource Source;

    public HasteItem(string path, HasteSource source) {
      Path = path;
      Source = source;
    }

    public HasteItem(HasteResult result) {
      Path = result.Path;
      Source = result.Source;
    }
  }
}