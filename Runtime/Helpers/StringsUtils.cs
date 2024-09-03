using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class StringsUtils
    {
        public static bool TextContainsSubtext(string text, string subText)
        {
            return text.ToLower().Contains(subText.ToLower());
        }
    }
}
