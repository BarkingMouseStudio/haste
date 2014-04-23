using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteResult {
    public string Path;
    public HasteSource Source;
    public int Score;

    public HasteResult(string path, HasteSource source, int score = 0) {
      Path = path;
      Source = source;
      Score = score;
    }

    public HasteResult(HasteItem item) {
      Path = item.Path;
      Source = item.Source;
      Score = 0;
    }
  }
}