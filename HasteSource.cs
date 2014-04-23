using System;

namespace Haste {

  [Flags]
  public enum HasteSource {
    Project = 1,
    Hierarchy = 2,
    Editor = 4
  }
}
