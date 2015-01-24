using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Haste {

  public class HasteBlur : IDisposable {

    const int passes = 10;

    Material blurMaterial;
    RenderTexture destTexture;

    public HasteBlur(int width, int height, Color tint) {
      blurMaterial = new Material(Shader.Find("Hidden/Haste/Blur"));
      blurMaterial.SetColor("_Tint", tint);
      blurMaterial.SetFloat("_Tinting", 0.4f);
      blurMaterial.SetFloat("_BlurSize", 2.0f);

      destTexture = new RenderTexture(width, height, 0);
      destTexture.Create();
    }

    public Texture BlurTexture(Texture sourceTexture) {
      RenderTexture active = RenderTexture.active; // Save original RenderTexture

      try {
        RenderTexture tempA = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
        RenderTexture tempB = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);

        for (int i = 0; i < passes; i++) {
          if (i == 0) {
            Graphics.Blit(sourceTexture, tempA, blurMaterial, 0);
          } else {
            Graphics.Blit(tempB, tempA, blurMaterial, 0);
          }
          Graphics.Blit(tempA, tempB, blurMaterial, 1);
        }

        Graphics.Blit(tempB, destTexture, blurMaterial, 2);

        Texture.DestroyImmediate(sourceTexture);

        RenderTexture.ReleaseTemporary(tempA);
        RenderTexture.ReleaseTemporary(tempB);
      } catch (Exception e) {
        Debug.LogException(e);
      } finally {
        RenderTexture.active = active; // Restore
      }

      return destTexture;
    }

    public void Dispose() {
      Material.DestroyImmediate(blurMaterial);
      RenderTexture.DestroyImmediate(destTexture);

      blurMaterial = null;
      destTexture = null;
    }
  }
}