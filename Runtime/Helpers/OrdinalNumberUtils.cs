namespace ReupVirtualTwin.helpers
{
    public static class OrdinalNumberUtils
    {
        public static string GetOrdinal(int number)
        {
            if (number <= 0)
            {
                return number.ToString();
            }
            int lastDigit = number % 10;
            int lastTwoDigits = number % 100;
            string suffix;
            if (lastTwoDigits >= 11 && lastTwoDigits <= 13)
            {
                suffix = "th";
            }
            else
            {
                switch (lastDigit)
                {
                    case 1:
                        suffix = "st";
                        break;
                    case 2:
                        suffix = "nd";
                        break;
                    case 3:
                        suffix = "rd";
                        break;
                    default:
                        suffix = "th";
                        break;
                }
            }
            return number + suffix;
        }
    }
}