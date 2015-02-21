using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public interface IHasteResult {
    HasteItem Item { get; }
    List<int> Indices { get; }
    bool IsDraggable { get; }
    bool IsSelected { get; }
    UnityEngine.Object Object { get; }
    string DragLabel { get; }
    float Score { get; }

    void Draw(bool isHighlighted);
    float Height(bool isHighlighted);
    bool Validate();
    void Action();
  }
}
