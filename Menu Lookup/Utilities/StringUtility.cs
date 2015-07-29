using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Menu_Lookup.Utilities
{
  public static class StringUtility
  {
    public static string UppercaseFirst(this string s)
    {
      // Check for empty string.
      if (string.IsNullOrEmpty(s))
      {
        return string.Empty;
      }
      // Return char and concat substring.
      return char.ToUpper(s[0]) + s.Substring(1);
    }

    public static bool IsNullOrTrimmedEmpty(this string s)
    {
      if (s == null) return true;
      if (s.Trim() == string.Empty) return true;
      return false; 
    }
  }
}
