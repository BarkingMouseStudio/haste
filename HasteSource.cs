using System;

namespace Haste {

  [Flags]
  public enum HasteSource {
    Unknown = 0,
    Project = 1,
    Hierarchy = 2,
    Editor = 4
  }
}
