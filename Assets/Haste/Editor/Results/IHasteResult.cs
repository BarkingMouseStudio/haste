using System;

namespace Haste {

  public interface IHasteResult : IComparable<IHasteResult> {
    HasteItem Item { get; }
    float Score { get; }

    bool IsVisible { get; set; }

    int[] Indices { get; }

    bool IsDraggable { get; }
    bool IsSelected { get; }
    UnityEngine.Object Object { get; }
    string DragLabel { get; }

    void Draw(bool isHighlighted);
    float Height(bool isHighlighted);
    bool Validate();
    void Action();
    void Select();
  }
}
