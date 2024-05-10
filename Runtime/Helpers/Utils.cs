using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class Utils
    {
        public static List<string> ConvertStringToList(string idsString)
        {
            string[] idsArray;
           if (!string.IsNullOrEmpty(idsString))
            {
                return new List<string>(idsString.Split(','));
            }
            else
            {
                return new List<string>();
            }

        }

        public static Color? ParseColor(string colorString)
        {
            if (ColorUtility.TryParseHtmlString(colorString, out Color parsedColor))
            {
                return parsedColor;
            }
            return null;

        }
    }
}
