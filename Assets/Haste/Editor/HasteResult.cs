using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteResult {
    public string Path;
    public int InstanceId;
    public HasteSource Source;
    public float Score;
    public IList<int> Indices;

    public HasteResult(string path, int instanceId, HasteSource source, float score = 0) {
      Path = path;
      InstanceId = instanceId;
      Source = source;
      Score = score;
      Indices = new List<int>();
    }

    public HasteResult(string path, int instanceId, HasteSource source, IList<int> indices, float score = 0) {
      Path = path;
      InstanceId = instanceId;
      Source = source;
      Score = score;
      Indices = indices;
    }

    public HasteResult(HasteItem item, float score = 0) {
      Path = item.Path;
      InstanceId = item.InstanceId;
      Source = item.Source;
      Score = score;
      Indices = new List<int>();
    }

    public HasteResult(HasteItem item, IList<int> indices, float score = 0) {
      Path = item.Path;
      InstanceId = item.InstanceId;
      Source = item.Source;
      Score = score;
      Indices = indices;
    }
  }
}