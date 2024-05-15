using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.helpers
{
    public static class DataManipulationHelpers
    {
        public static object GetValueAtPath(this Dictionary<string, object> dictionary, string[] path)
        {
            object current = dictionary;
            foreach (var key in path)
            {
                if (current is Dictionary<string, object> currentDict)
                {
                    if (!currentDict.TryGetValue(key, out current))
                    {
                        return null;
                    }
                }
                else if (current is JObject currentJObject)
                {
                    current = currentJObject[key];
                    if (current == null)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return CastValue(current);
        }
    private static object CastValue(object input)
    {
        switch (input)
        {
            case JObject jObject:
                return jObject.ToObject<Dictionary<string, object>>();
            case JValue jValue:
                return ConvertJValue(jValue);
            default:
                return input;
        }
    }

private static object ConvertJValue(JValue jValue)
    {
        switch (jValue.Type)
        {
            case JTokenType.String:
                return jValue.Value<string>();
            case JTokenType.Integer:
                return jValue.Value<int>();
            case JTokenType.Float:
                return jValue.Value<float>();
            case JTokenType.Boolean:
                return jValue.Value<bool>();
            default:
                return jValue.Value;
        }
    }
    }
}
