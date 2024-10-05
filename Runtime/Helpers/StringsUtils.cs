using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class StringsUtils
    {
        public static bool TextContainsSubtext(string text, string subText)
        {
            return text.ToLower().Contains(subText.ToLower());
        }
        public static bool MatchRegex(string tagName, string regex)
        {
            return Regex.IsMatch(tagName, regex);
        }
    }
}
