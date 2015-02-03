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

    public HasteHierarchyResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    GUIStyle GetLabelStyle(GameObject go) {
      var prefabType = PrefabUtility.GetPrefabType(go);
      if (prefabType == PrefabType.PrefabInstance || prefabType == PrefabType.ModelPrefabInstance) {
        return HasteStyles.PrefabStyle;
      } else if (prefabType == PrefabType.MissingPrefabInstance) {
        return HasteStyles.BrokenPrefabStyle;
      } else if (!go.activeInHierarchy) {
        return HasteStyles.DisabledNameStyle;
      } else {
        return HasteStyles.NameStyle;
      }
    }

    public override void Draw(bool isHighlighted) {
      var go = (GameObject)Object;

      var rect = EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32));
      rect.y += 5; // center the icon vertically
      GUI.DrawTexture(rect, GameObjectIcon);

      using (new HasteVertical()) {
        var childCount = go.transform.childCount;
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
      Selection.activeInstanceID = Item.Id;
      EditorGUIUtility.PingObject(Selection.activeInstanceID);
    }

    public override void Select() {
      Selection.activeInstanceID = Item.Id;
    }
  }
}