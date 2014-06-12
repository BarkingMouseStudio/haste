using System;
using UnityEngine;
 
namespace Haste {

  public class StackBlur {

    public static Color[] Blur(Color[] pixels, Rect dims, int radius, float tint = 0.0f) {
      if (radius < 1) return pixels;

      int width = (int)dims.width;
      int height = (int)dims.height;

      var dest = new Color[width * height];
      var source = new int[width * height];

      int i;
      for (i = 0; i < source.Length; i++) {
        source[i] = (int)(
          0xff000000u |
            (uint)((int)(Mathf.Max(Mathf.Min(pixels[i].r + tint, 1.0f), 0.0f) * 255) << 16) |
            (uint)((int)(Mathf.Max(Mathf.Min(pixels[i].g + tint, 1.0f), 0.0f) * 255) << 8) |
            (uint)((int)(Mathf.Max(Mathf.Min(pixels[i].b + tint, 1.0f), 0.0f) * 255))
          );
      }
   
      int w = width;
      int h = height;
      int wm = w - 1;
      int hm = h - 1;
      int wh = w * h;
      int div = radius + radius + 1;
      var r = new int[wh];
      var g = new int[wh];
      var b = new int[wh];
      int rsum, gsum, bsum, x, y, p1, p2, yi;
      var vmin = new int[Math.Max(w, h)];
      var vmax = new int[Math.Max(w, h)];
   
      var dv = new int[256 * div];
      for (i = 0; i < 256 * div; i++) {
        dv[i] = (i / div);
      }
   
      int yw = yi = 0;
   
      for (y = 0; y < h; y++) { // blur horizontal
        rsum = gsum = bsum = 0;

        for (i = -radius; i <= radius; i++) {
          int p = source[yi + Math.Min(wm, Math.Max(i, 0))];
          rsum += (p & 0xff0000) >> 16;
          gsum += (p & 0x00ff00) >> 8;
          bsum += (p & 0x0000ff);
        }

        for (x = 0; x < w; x++) {
          r[yi] = dv[rsum];
          g[yi] = dv[gsum];
          b[yi] = dv[bsum];
   
          if (y == 0) {
            vmin[x] = Math.Min(x + radius + 1, wm);
            vmax[x] = Math.Max(x - radius, 0);
          }

          p1 = source[yw + vmin[x]];
          p2 = source[yw + vmax[x]];
   
          rsum += ((p1 & 0xff0000) - (p2 & 0xff0000)) >> 16;
          gsum += ((p1 & 0x00ff00) - (p2 & 0x00ff00)) >> 8;
          bsum += (p1 & 0x0000ff) - (p2 & 0x0000ff);

          yi++;
        }

        yw += w;
      }
   
      for (x = 0; x < w; x++) { // blur vertical
        rsum = gsum = bsum = 0;
        int yp = -radius * w;

        for (i = -radius; i <= radius; i++) {
          yi = Math.Max(0, yp) + x;

          rsum += r[yi];
          gsum += g[yi];
          bsum += b[yi];

          yp += w;
        }

        yi = x;

        for (y = 0; y < h; y++) {
          dest[yi] = new Color(dv[rsum] / 255.0f, dv[gsum] / 255.0f, dv[bsum] / 255.0f);

          if (x == 0) {
            vmin[y] = Math.Min(y + radius + 1, hm) * w;
            vmax[y] = Math.Max(y - radius, 0) * w;
          }

          p1 = x + vmin[y];
          p2 = x + vmax[y];
   
          rsum += r[p1] - r[p2];
          gsum += g[p1] - g[p2];
          bsum += b[p1] - b[p2];
   
          yi += w;
        }
      }

      return dest;
    }
  }
}
