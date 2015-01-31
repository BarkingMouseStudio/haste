using UnityEditor;
using UnityEngine;
using System;
using System.Collections;

namespace Haste {

  // Utility for using textures and disposing of them:
  //
  // using (var texture = new HasteTexture(someTexture)) {
  //   Debug.Log(texture.Tex);
  // }
  public class HasteTexture : IDisposable {

    public Texture Tex { get; protected set; }

    public HasteTexture(Texture texture) {
      Tex = texture;
    }

    public void Dispose() {
      Texture.DestroyImmediate(Tex);
    }
  }
}