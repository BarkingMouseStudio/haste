using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public interface IHasteResult {
    IHasteItem Item { get; }

    string Name { get; }
    string NameBoundaries { get; }

    bool IsFirstCharMatch { get; }
    bool IsFirstCharNameMatch { get; }

    bool IsPrefixMatch { get; }
    bool IsNamePrefixMatch { get; }

    bool IsExactMatch { get; }
    bool IsExactNameMatch { get; }

    float BoundaryQueryRatio { get; }
    float BoundaryUtilization { get; }

    int[] Indices { get; }

    bool IsDraggable { get; }
    bool IsSelected { get; }
    UnityEngine.Object Object { get; }
    string DragLabel { get; }

    void Draw(bool isHighlighted, bool highlightMatches);
    float Height(bool isHighlighted);
    bool Validate();
    void Action();
  }
}