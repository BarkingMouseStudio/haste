using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public class HasteHierarchyResult : AbstractHasteResult {

    private static Texture _GameObjectIcon;
    public static Texture GameObjectIcon {
      get {
        if (_GameObjectIcon == null) {
          _GameObjectIcon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
        }
        return _GameObjectIcon;
      }
    }

    private UnityEngine.Object object_;
    public override UnityEngine.Object Object {
      get {
        if (object_ == null) {
          object_ = EditorUtility.InstanceIDToObject(Item.Id);
        }
        return object_;
      }
    }

    public override bool IsDraggable {
      get { return true; }
    }

    public override string DragLabel {
      get { return Object.name; }
    }

    public HasteHierarchyResult(HasteItem item) : base(item) {}

    GUIStyle GetLabelStyle(GameObject go) {
      switch (PrefabUtility.GetPrefabType(go)) {
        case PrefabType.PrefabInstance:
        case PrefabType.ModelPrefabInstance:
          if (go.activeInHierarchy) {
            return HasteStyles.PrefabStyle;
          } else {
            return HasteStyles.DisabledPrefabStyle;
          }
        case PrefabType.MissingPrefabInstance:
          if (go.activeInHierarchy) {
            return HasteStyles.BrokenPrefabStyle;
          } else {
            return HasteStyles.DisabledBrokenPrefabStyle;
          }
        default:
          if (go.activeInHierarchy) {
            return HasteStyles.NameStyle;
          } else {
            return HasteStyles.DisabledNameStyle;
          }
      }
    }

    public override void Draw(bool isHighlighted) {
      var go = (GameObject)Object;

      var rect = EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32));
      rect.y += 5; // center the icon vertically
      GUI.DrawTexture(rect, GameObjectIcon);

      using (new HasteVertical()) {
        var childCount = go.transform != null ? go.transform.childCount : 0;
        if (childCount > 0) {
          EditorGUILayout.LabelField(String.Format("{0} ({1})", Path.GetFileName(Item.Path), childCount), isHighlighted ? HasteStyles.HighlightedNameStyle : GetLabelStyle(go));
        } else {
          EditorGUILayout.LabelField(Path.GetFileName(Item.Path), isHighlighted ? HasteStyles.HighlightedNameStyle : GetLabelStyle(go));
        }
        EditorGUILayout.LabelField(HasteUtils.BoldLabel(Item.Path, Indices.ToArray(), isHighlighted ? HasteStyles.HighlightedBoldStart : HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.HighlightedDescriptionStyle : HasteStyles.DescriptionStyle);
      }
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.instanceIDs = new int[]{Item.Id};
      EditorGUIUtility.PingObject(Selection.activeInstanceID);
    }
  }
}