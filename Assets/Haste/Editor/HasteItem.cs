using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public struct HasteItem {
    public string Path;
    public int InstanceId;
    public HasteSource Source;
    public Texture Icon;

    public HasteItem(string path, int instanceId, HasteSource source, Texture icon = null) {
      Path = path;
      Source = source;
      InstanceId = instanceId;
      Icon = icon;
    }

    public HasteItem(HasteResult result) {
      Path = result.Path;
      Source = result.Source;
      InstanceId = result.InstanceId;
      Icon = result.Icon;
    }
  }
}