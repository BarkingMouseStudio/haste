using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteList : ScriptableObject {

    Vector2 scrollPosition = Vector2.zero;
    int highlightedIndex = 0;

    private IHasteResult[] items;
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
        if (highlightedIndex < 0 || highlightedIndex >= Items.Length) {
          return null;
        }
        return Items[highlightedIndex];
      }
    }

    void ResetScroll() {
      scrollPosition = Vector2.zero;
    }

    void ScrollTo(int toIndex) {
      var heightOffset = 0.0f;
      for (var i = 0; i < toIndex; i++) {
        if (Items[i] != null) {
          heightOffset += Items[i].Height(i == highlightedIndex);
        }
      }
      scrollPosition = new Vector2(scrollPosition.x, heightOffset);
    }

    public void OnUpArrow() {
      int index = Mathf.Max(highlightedIndex - 1, 0);
      SetHighlightedIndex(index);
    }

    public void OnDownArrow() {
      int index = Mathf.Min(highlightedIndex + 1, Items.Length - 1);
      SetHighlightedIndex(index);
    }

    // void OnMouseDrag(Event e) {
    //   // TODO: highlightedResult is wrong
    //   DragAndDrop.PrepareStartDrag();
    //   DragAndDrop.objectReferences = new UnityEngine.Object[]{highlightedResult.Object};
    //   DragAndDrop.StartDrag(highlightedResult.DragLabel);
    //   Event.current.Use();
    // }

    void OnKeyDown(Event e) {
      switch (e.keyCode) {
        case KeyCode.UpArrow:
          e.Use();
          OnUpArrow();
          break;
        case KeyCode.DownArrow:
          e.Use();
          OnDownArrow();
          break;
      }
    }

    void SetHighlightedIndex(int index) {
      highlightedIndex = Mathf.Clamp(index, 0, Items.Length - 1);
      if (HighlightedItem != null) {
        HighlightedItem.Select();
      }
      ScrollTo(index);
    }

    void OnEvent(Event e) {
      switch (e.type) {
        case EventType.KeyDown:
          OnKeyDown(e);
          break;
      }
    }

    public void OnGUI() {
      using (var scrollView = new HasteScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true))) {
        scrollPosition = scrollView.ScrollPosition;

        bool isHighlighted;
        for (var i = 0; i < Items.Length; i++) {
          isHighlighted = i == highlightedIndex;
          HasteListItem.Draw(Items[i], isHighlighted);
        }
      }
    }
  }
}
