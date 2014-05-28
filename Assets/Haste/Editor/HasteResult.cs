using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Haste {

  public interface IHasteResult {
    void Draw();
    bool Validate();
    void Action();
  }

  public abstract class AbstractHasteResult {
    public virtual bool Validate() {
      return true;
    }

    public virtual void Draw() {
      if (result.Icon != null) {
        GUI.DrawTexture(EditorGUILayout.GetControlRect(GUILayout.Width(32), GUILayout.Height(32)), result.Icon);
      }

      #if IS_HASTE_PRO
        // string description = result.Description;
      #else
        string description = "Download Haste Pro from the Unity Asset Store (Disabled)";
      #endif

      EditorGUILayout.BeginVertical();
      EditorGUILayout.LabelField(Path.GetFileName(result.Path), index == highlightedIndex ? highlightStyle : nameStyle);
      EditorGUILayout.LabelField(BoldLabel(result.Path, result.Indices.ToArray()), descriptionStyle);
      EditorGUILayout.LabelField(result.Description, descriptionStyle);
      EditorGUILayout.EndVertical();
    }

    public virtual void Action() {
    }
  }

  public struct HasteResult {
    public string Path;
    public int InstanceId;
    public HasteSource Source;
    public float Score;
    public IList<int> Indices;
    public Texture Icon;
    public string Description;

    public HasteResult(string path, int instanceId, HasteSource source, float score = 0, Texture icon = null) {
      Path = path;
      InstanceId = instanceId;
      Source = source;
      Score = score;
      Indices = new List<int>();
      Icon = icon;
      Description = "";
    }

    public HasteResult(string path, int instanceId, HasteSource source, IList<int> indices, float score = 0, Texture icon = null) {
      Path = path;
      InstanceId = instanceId;
      Source = source;
      Score = score;
      Indices = indices;
      Icon = icon;
      Description = "";
    }

    public HasteResult(HasteItem item, float score = 0) {
      Path = item.Path;
      InstanceId = item.InstanceId;
      Source = item.Source;
      Icon = item.Icon;
      Score = score;
      Indices = new List<int>();
      Description = "";
    }

    public HasteResult(HasteItem item, IList<int> indices, float score = 0) {
      Path = item.Path;
      InstanceId = item.InstanceId;
      Source = item.Source;
      Score = score;
      Indices = indices;
      Icon = item.Icon;
      Description = "";
    }
  }
}