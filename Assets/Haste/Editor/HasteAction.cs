using System;

namespace Haste {

  public struct HasteAction {
    public string Name;
    public string Description;
    public Action<HasteResult> Action;

    public HasteAction(string name, string description, Action<HasteResult> action) {
      Name = name;
      Description = description;
      Action = action;
    }
  }
}