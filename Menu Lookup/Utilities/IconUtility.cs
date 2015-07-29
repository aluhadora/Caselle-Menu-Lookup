using System.Drawing;

namespace Menu_Lookup.Utilities
{
  public static class IconUtility
  {
    public static Bitmap IconForPath(string filePath)
    {
      using(var icon = Icon.ExtractAssociatedIcon(filePath))
      using (var icon2 = icon != null ? new Icon(icon, new Size(16, 16)) : null)
      {
        if (icon2 == null) return null;
        return icon2.ToBitmap();
      }
    }
  }
}
