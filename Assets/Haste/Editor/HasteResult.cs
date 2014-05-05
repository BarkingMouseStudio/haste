using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteResult {
    public string Path;
    public HasteSource Source;
    public float Score;
    public IList<int> Indices;

    public HasteResult(string path, HasteSource source, float score = 0) {
      Path = path;
      Source = source;
      Score = score;
      Indices = new List<int>();
    }

    public HasteResult(string path, HasteSource source, IList<int> indices, float score = 0) {
      Path = path;
      Source = source;
      Score = score;
      Indices = indices;
    }

    public HasteResult(HasteItem item, float score = 0) {
      Path = item.Path;
      Source = item.Source;
      Score = score;
      Indices = new List<int>();
    }

    public HasteResult(HasteItem item, IList<int> indices, float score = 0) {
      Path = item.Path;
      Source = item.Source;
      Score = score;
      Indices = indices;
    }
  }
}