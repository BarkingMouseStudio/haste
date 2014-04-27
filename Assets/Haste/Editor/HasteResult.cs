using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteResult {
    public string Path;
    public HasteSource Source;
    public float Score;

    public HasteResult(string path, HasteSource source, float score = 0) {
      Path = path;
      Source = source;
      Score = score;
    }

    public HasteResult(HasteItem item, float score = 0) {
      Path = item.Path;
      Source = item.Source;
      Score = score;
    }
  }
}