using UnityEngine;
using UnityEditor;

namespace Haste {

  public struct Item {
    public string Name;
    public string Path;
    public Source Source;
    public Texture Icon;

    public Item(string name, string path, Source source, Texture icon) {
      this.Name = name;
      this.Path = path;
      this.Source = source;
      this.Icon = icon;
    }

    public Item(string name, string path, Source source) {
      this.Name = name;
      this.Path = path;
      this.Source = source;
      this.Icon = EditorGUIUtility.ObjectContent(null, typeof(GameObject)).image;
    }
  }
}