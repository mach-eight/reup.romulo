using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.helpers
{
    public static class TypeHelpers
    {
        public static bool NotNull(JToken jToken)
        {
            return jToken != null && jToken.Type != JTokenType.Null;
        }
    }
}
