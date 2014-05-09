using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteItem {
    public string Path;
    public int InstanceId;
    public HasteSource Source;

    public HasteItem(string path, int instanceId, HasteSource source) {
      Path = path;
      Source = source;
      InstanceId = instanceId;
    }

    public HasteItem(HasteResult result) {
      Path = result.Path;
      Source = result.Source;
      InstanceId = result.InstanceId;
    }
  }
}