using System;
using System.Windows.Media;

namespace Imaging
{
  public static class ColorExtensions
  {
    static void GetKoefs(Color color, out float rKoef, out float gKoef, out float bKoef)
    {
      rKoef = ((float)color.R) / 255f;
      gKoef = ((float)color.G) / 255f;
      bKoef = ((float)color.B) / 255f;
    }

    public static double GetHue(this Color color)
    {
      if (color.R == color.G && color.G == color.B)
        return 0f;

      float rKoef, gKoef, bKoef;
      GetKoefs(color, out rKoef, out gKoef, out bKoef);

      float maxRGB, minRGB;
      GetMinMaxRGB(rKoef, gKoef, bKoef, out maxRGB, out minRGB);
      float diff = maxRGB - minRGB;

      float result = 0f;
      if (rKoef == maxRGB)
        result = (gKoef - bKoef) / diff;
      else if (gKoef == maxRGB)
        result = 2f + ((bKoef - rKoef) / diff);
      else if (bKoef == maxRGB)
        result = 4f + ((rKoef - gKoef) / diff);
      result *= 60f;
      if (result < 0f)
        result += 360f;
      return result;
    }

    private static void GetMinMaxRGB(float rKoef, float gKoef, float bKoef, out float maxRGB, out float minRGB)
    {
      maxRGB = rKoef;
      maxRGB = Math.Max(gKoef, maxRGB);
      maxRGB = Math.Max(bKoef, maxRGB);
      minRGB = rKoef;
      minRGB = Math.Min(gKoef, minRGB);
      minRGB = Math.Min(bKoef, minRGB);
    }

    public static float GetSaturation(this Color color)
    {
      float rKoef, gKoef, bKoef;
      GetKoefs(color, out rKoef, out gKoef, out bKoef);

      float result = 0f;
      float maxRGB;
      float minRGB;
      GetMinMaxRGB(rKoef, gKoef, bKoef, out maxRGB, out minRGB);
      if (maxRGB == minRGB)
        return result;
      float brightness = (maxRGB + minRGB) / 2f;
      if (brightness <= 0.5)
        return (maxRGB - minRGB) / (maxRGB + minRGB);
      return (maxRGB - minRGB) / ((2f - maxRGB) - minRGB);
    }

    public static float GetBrightness(this Color color)
    {
      float rKoef = ((float)color.R) / 255f;
      float gKoef = ((float)color.G) / 255f;
      float bKoef = ((float)color.B) / 255f;
      float maxRGB;
      float minRGB;
      GetMinMaxRGB(rKoef, gKoef, bKoef, out maxRGB, out minRGB);
      return (maxRGB + minRGB) / 2f;
    }
  }
}
