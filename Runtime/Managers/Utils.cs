using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static List<string> ConvertStringToList(string idsString)
    {
        string[] idsArray;
        if (idsString.Length > 0)
        {
            idsArray = idsString.Split(',');
            List<string> idsList = new List<string>(idsArray);
            return idsList;
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
