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

    bool IsFirstCharMatch { get; }
    bool IsPrefixMatch { get; }
    bool IsNamePrefixMatch { get; }
    int IndexSum { get; }
    int GapSum { get; }
    int PathLen { get; }
    float BoundaryQueryRatio { get; }
    float BoundaryUtilization { get; }

    void Draw(bool isHighlighted);
    float Height(bool isHighlighted);
    bool Validate();
    void Action();
  }
}
