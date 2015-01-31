using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Haste {

  public class HasteGUIList<T> : ScriptableObject {

    Vector2 scrollPosition = Vector2.zero;

    public IHasteResult HighlightedItem { get; protected set; }
    public IHasteResult[] Items { get; set; }

    public int Size {
      get {
        if (Items == null) {
          return 0;
        } else {
          return Items.Length;
        }
      }
    }

    // int highlightedIndex = 0;

    // const int itemHeight = 46;
    // IHasteResult selectedResult;
    // IHasteResult highlightedResult;

    // public HasteGUIList(params GUILayoutOption[] options) {
    //   this.options = options;
    // }

    // void ResetScroll() {
    //   scrollPosition = Vector2.zero;
    // }

    // void UpdateScroll() {
    //   int highlightOffset = highlightedIndex * itemHeight;
    //   scrollPosition = new Vector2(scrollPosition.x, highlightOffset);
    // }

    // void SelectResult(IHasteResult result) {
    //   if (results.Length > 0 && highlightedIndex >= 0) {
    //     SelectResult(results[highlightedIndex]);
    //   }

    //   selectedResult = result;
    //   selectedResult.Action();
    // }

    // void OnUpArrow() {
    //   int index = Math.Max(highlightedIndex - 1, 0);
    //   SetHighlightedIndex(index);
    // }

    // void OnDownArrow() {
    //   int index = Math.Min(highlightedIndex + 1, results.Length - 1);
    //   SetHighlightedIndex(index);
    // }

    // void OnMouseDrag(Event e) {
    //   // TODO: highlightedResult is wrong
    //   DragAndDrop.PrepareStartDrag();
    //   DragAndDrop.objectReferences = new UnityEngine.Object[]{highlightedResult.Object};
    //   DragAndDrop.StartDrag(highlightedResult.DragLabel);
    //   Event.current.Use();
    // }

    // void OnKeyDown(Event e) {
    //   switch (e.keyCode) {
    //     case KeyCode.UpArrow:
    //       e.Use();
    //       OnUpArrow();
    //       break;
    //     case KeyCode.DownArrow:
    //       e.Use();
    //       OnDownArrow();
    //       break;
    //     }
    //   }
    // }

    // void OnEvent(Event e) {
    //   switch (e.type) {
    //     case EventType.MouseDrag:
    //       OnMouseDrag(e);
    //       break;
    //     case EventType.KeyDown:
    //       OnKeyDown(e);
    //       break;
    //     }
    //   }
    // }

    // void SetHighlightedIndex(int index, bool updateScroll = true) {
    //   highlightedIndex = index;

    //   if (highlightedIndex < 0 || highlightedIndex > results.Length - 1) {
    //     if (updateScroll) {
    //       ResetScroll();
    //     }
    //     return;
    //   }

    //   highlightedResult = results[highlightedIndex];
    //   highlightedResult.Select();

    //   if (updateScroll) {
    //     UpdateScroll();
    //   }
    // }

    public void OnGUI(params GUILayoutOption[] options) {
      using (var scrollView = new HasteScrollView(scrollPosition, options)) {
        scrollPosition = scrollView.ScrollPosition;

        for (int i = 0; i < Items.Length; i++) {
          // IHasteResult result = results[i];
          // DrawResult(result, i);
        }
      }
    }
  }
}
