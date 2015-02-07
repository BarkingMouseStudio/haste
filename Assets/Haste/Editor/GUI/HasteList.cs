using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteList : ScriptableObject {

    Vector2 scrollPosition = Vector2.zero;
    int highlightedIndex = 0;

    private IHasteResult[] items = new IHasteResult[0];
    public IHasteResult[] Items {
      get {
        return items;
      }
      set {
        items = value;
        SetHighlightedIndex(0);
      }
    }

    public int Size {
      get { return Items != null ? Items.Length : 0; }
    }

    public IHasteResult HighlightedItem {
      get {
        if (highlightedIndex >= 0 && highlightedIndex < Items.Length) {
          return Items[highlightedIndex];
        } else {
          return null;
        }
      }
    }

    void OnEnable() {
      base.hideFlags = HideFlags.HideAndDontSave;
    }

    void ResetScroll() {
      scrollPosition = Vector2.zero;
    }

    void ScrollTo(int toIndex) {
      if (toIndex >= 0 && toIndex < Items.Length) {
        var heightOffset = 0.0f;
        for (var i = 0; i < toIndex; i++) {
          if (Items[i] != null) {
            heightOffset += Items[i].Height(i == highlightedIndex);
          }
        }
        scrollPosition = new Vector2(scrollPosition.x, heightOffset);
      }
    }

    public void OnUpArrow() {
      if (Items != null) {
        int index = Mathf.Max(highlightedIndex - 1, 0);
        SetHighlightedIndex(index);
      }
    }

    public void OnDownArrow() {
      if (Items != null) {
        int index = Mathf.Min(highlightedIndex + 1, Items.Length - 1);
        SetHighlightedIndex(index);
      }
    }

    // void OnMouseDrag(Event e) {
    //   // TODO: highlightedResult is wrong
    //   DragAndDrop.PrepareStartDrag();
    //   DragAndDrop.objectReferences = new UnityEngine.Object[]{highlightedResult.Object};
    //   DragAndDrop.StartDrag(highlightedResult.DragLabel);
    //   Event.current.Use();
    // }

    void SetHighlightedIndex(int index) {
      highlightedIndex = Mathf.Clamp(index, 0, Items.Length - 1);
      ScrollTo(index);
    }

    public void OnGUI() {
      using (var scrollView = new HasteScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        scrollPosition = scrollView.ScrollPosition;

        bool isHighlighted;
        for (var i = 0; i < Items.Length; i++) {
          isHighlighted = i == highlightedIndex;
          var e = HasteListItem.Draw(Items[i], isHighlighted);
          switch (e) {
            case HasteListItemEvent.DoubleClick:
              break;
            case HasteListItemEvent.Click:
              break;
            case HasteListItemEvent.MouseDown:
              break;
          }
        }
      }
    }
  }
}
