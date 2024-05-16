using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using UnityEngine;
using System;

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
                case JArray jArray:
                    return ConvertJArray(jArray);
                case JValue jValue:
                    return ConvertJValue(jValue);
                default:
                    return ForceInt32AndFloatTypes(input);
            }
        }

        private static object ConvertJArray(JArray jArray)
        {
            var isStringArray = jArray.All(x => x.Type == JTokenType.String);
            var isIntArray = jArray.All(x => x.Type == JTokenType.Integer);
            var isFloatArray = jArray.All(x => x.Type == JTokenType.Float);

            if (isStringArray)
                return jArray.ToObject<string[]>();
            else if (isIntArray)
                return jArray.ToObject<int[]>();
            else if (isFloatArray)
                return jArray.ToObject<float[]>();
            else
                return jArray.ToObject<object[]>();
        }
        private static object ConvertJValue(JValue jValue)
        {
            switch (jValue.Type)
            {
                case JTokenType.String:
                    return jValue.Value<string>();
                case JTokenType.Integer:
                    return (jValue.Value<int>());
                case JTokenType.Float:
                    return jValue.Value<float>();
                case JTokenType.Boolean:
                    return jValue.Value<bool>();
                default:
                    return jValue.Value;
            }
        }

        // Todo: We should not need this method here.
        // We should be able to use the JsonConverter in NewtonSoft to handle this types by default.
        private static object ForceInt32AndFloatTypes(object value)
        {
            if (value is Int64)
            {
                return Convert.ToInt32(value);
            }
            if (value is Double)
            {
                return Convert.ToSingle(value);
            }
            return value;
        }
    }
}
