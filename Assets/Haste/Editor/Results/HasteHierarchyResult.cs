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

    public HasteHierarchyResult(HasteItem item, float score, List<int> indices) : base(item, score, indices) {}

    UnityEngine.Object CurrentObject {
      get {
        return EditorUtility.InstanceIDToObject(Item.Id);
      }
    }

    GameObject CurrentGameObject {
      get {
        return (GameObject)CurrentObject;
      }
    }

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
      var go = CurrentGameObject;

      GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)),
        GameObjectIcon);

      using (new HasteVertical()) {
        var childCount = go.transform.childCount;
        if (childCount > 0) {
          EditorGUILayout.LabelField(String.Format("{0} ({1})", Path.GetFileName(Item.Path), childCount), isHighlighted ? HasteStyles.HighlightedNameStyle : GetLabelStyle(go));
        } else {
          EditorGUILayout.LabelField(Path.GetFileName(Item.Path), isHighlighted ? HasteStyles.HighlightedNameStyle : GetLabelStyle(go));
        }
        EditorGUILayout.LabelField(HasteUtils.BoldLabel(Item.Path, Indices.ToArray(), HasteStyles.BoldStart, HasteStyles.BoldEnd), isHighlighted ? HasteStyles.HighlightedDescriptionStyle : HasteStyles.DescriptionStyle);
      }
    }

    public override void Action() {
      EditorApplication.ExecuteMenuItem("Window/Hierarchy");
      Selection.activeInstanceID = Item.Id;
    }

    public override void Select() {
      Selection.activeInstanceID = Item.Id;
    }
  }
}