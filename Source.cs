using System;

namespace Haste {

  [Flags]
  public enum Source {
    Project = 1,
    Hierarchy = 2,
    Editor = 4
  }
}
