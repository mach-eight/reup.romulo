using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class DataValidator
    {
        public const string boolType = "bool";
        public const string intType = "int";
        public const string stringType = "string";
        public const string objectType = "object";
        public const string arrayType = "array";

        public static readonly JObject stringSchema = new()
        {
            { "type", stringType }
        };
        public static readonly JObject intSchema = new JObject
        {
            { "type", intType }
        };
        public static readonly JObject boolSchema = new JObject
        {
            { "type", boolType }
        };

        static public JObject CreateArraySchema(JObject[] itemSchemas)
        {
            return new JObject
            {
                { "type", arrayType },
                { "items", new JArray ( itemSchemas ) }
            };
        }

        static public bool ValidateObjectToSchema(object obj, JObject schemaKeys)
        {
            string json = JsonConvert.SerializeObject(obj);
            return ValidateJsonStringToSchema(json, schemaKeys);
        }

        static public bool ValidateJsonStringToSchema(string json, JObject schema)
        {
            JToken jsonObj = JsonConvert.DeserializeObject<JToken>(json);
            return ValidateJTokenToSchema(jsonObj, schema);
        }

        static private bool ValidateJTokenToSchema(JToken obj, JObject schema)
        {
            return ValidateJTokenToSchema(obj, schema, "");
        }

        static private bool ValidateJTokenToSchema(JToken obj, JObject schema, string key)
        {
            switch ((string)schema["type"])
            {
                case boolType:
                    return ValidateJObjectType(obj, JTokenType.Boolean, key);
                case intType:
                    return ValidateJObjectType(obj, JTokenType.Integer, key);
                case stringType:
                    return ValidateJObjectType(obj, JTokenType.String, key);
                case objectType:
                    return ValidateJObjectProperties(obj, (JObject)schema["properties"]);
                case arrayType:
                    return ValidateJArrayItems(obj, (JArray)schema["items"]);
                default:
                    Debug.LogWarning($"Type {schema["type"]} not supported");
                    return false;
            }
        }
        static private bool ValidateJArrayItems(JToken array, JArray acceptedSchemas)
        {
            if (!ValidateJObjectType(array, JTokenType.Array))
            {
                return false;
            }
            foreach (JToken item in array)
            {
                if (!ValidateJTokenToAnySchema(item, acceptedSchemas))
                {
                    Debug.LogWarning($"Validation of item {item} failed");
                    return false;
                }
            }
            return true;
        }
        static private bool ValidateJTokenToAnySchema(JToken obj, JArray schemas)
        {
            foreach (JToken schema in schemas)
            {
                if (ValidateJTokenToSchema(obj, (JObject)schema))
                {
                    return true;
                }
            }
            return false;
        }
        static private bool ValidateJObjectProperties(JToken obj, JObject properties)
        {
            if (!ValidateJObjectType(obj, JTokenType.Object))
            {
                return false;
            }
            foreach (KeyValuePair<string, JToken> pair in properties)
            {
                if (obj[pair.Key] == null)
                {
                    Debug.LogWarning($"Key {pair.Key} not found in object");
                    return false;
                }
                if (!ValidateJTokenToSchema(obj[pair.Key], (JObject)pair.Value, pair.Key))
                {
                    Debug.LogWarning($"Validation of key {pair.Key} failed");
                    return false;
                }
            }
            return true;
        }

        static private bool ValidateJObjectType(JToken obj, JTokenType expectedType, string key)
        {
            bool valid = ValidateJObjectType(obj, expectedType);
            if (!valid && !string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"Key {key} is not of type {expectedType}");
            }
            return valid;
        }

        static private bool ValidateJObjectType(JToken obj, JTokenType expectedType)
        {
            if (obj.Type != expectedType)
            {
                Debug.LogWarning($"Expected object to be of type {expectedType}, but actual type is {obj.Type}");
                return false;
            }
            return true;
        }

    }
}
