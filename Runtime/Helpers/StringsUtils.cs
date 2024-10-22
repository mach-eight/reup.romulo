using System.Text.RegularExpressions;

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
            try
            {
                return Regex.IsMatch(tagName, regex);
            }
            catch
            {
                return false;
            }
        }
    }
}
