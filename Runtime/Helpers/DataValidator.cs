using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class DataValidator
    {
        public const string intType = "int";
        public const string stringType = "string";
        public const string objectType = "object";
        public const string arrayType = "array";

        public static bool ValidateObjectToSchema(object obj, JObject schemaKeys)
        {
            string json = JsonConvert.SerializeObject(obj);
            return ValidateJsonStringToSchema(json, schemaKeys);
        }

        public static bool ValidateJsonStringToSchema(string json, JObject schema)
        {
            JObject jsonObj = JObject.Parse(json);
            return ValidateJTokenToSchema(jsonObj, schema);
        }

        static private bool ValidateJTokenToSchema(JToken obj, JObject schema)
        {
            return ValidateJTokenToSchema(obj, schema, "");
        }

        static private bool ValidateJTokenToSchema(JToken obj, JObject schema, string key)
        {
            Debug.Log("expected type is " + schema["type"]);
            switch ((string)schema["type"])
            {
                case intType:
                    Debug.Log("it was int type");
                    return ValidateJObjectType(obj, JTokenType.Integer, key);
                case stringType:
                    Debug.Log("it was string type");
                    return ValidateJObjectType(obj, JTokenType.String, key);
                case objectType:
                    Debug.Log("it was object type");
                    return ValidateJObjectProperties((JObject)obj, (JObject)schema["properties"]);
                default:
                    Debug.LogWarning($"Type {schema["type"]} not supported");
                    return false;
            }
        }
        static private bool ValidateJObjectProperties(JObject obj, JObject properties)
        {
            foreach (KeyValuePair<string, JToken> pair in properties)
            {
                Debug.Log("validating key " + pair.Key);
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
                return false;
            }
            return true;
        }


        //static private bool ValidateObjectKeyType(JObject obj, string key, JObject schema)
        //{
        //    Debug.Log("validating key " + key + " with schema " + schema["type"]);
        //    switch ((string)schema["type"])
        //    {
        //        case intType:
        //            Debug.Log("it was int type");
        //            return ValidateObjectKeyType(obj, key, JTokenType.Integer);
        //        case stringType:
        //            Debug.Log("it was string type");
        //            return ValidateObjectKeyType(obj, key, JTokenType.String);
        //        case objectType:
        //            return ValidateJsonStringToSchema(
        //                obj[key].ToString(),
        //                (JObject)schema["properties"]);
        //        default:
        //            Debug.LogWarning($"Type {schema["type"]} not supported");
        //            return false;
        //    }
        //}

        //static private bool ValidateObjectKeyType(JObject obj, string key, JTokenType expectedType)
        //{
        //    if (obj[key].Type != expectedType)
        //    {
        //        Debug.LogWarning($"Key {key} is not of type {expectedType}");
        //        return false;
        //    }
        //    return true;
        //}

        //static private bool TypeIsNestedObject(object type)
        //{
        //    return type is Dictionary<string, object>;
        //}

        //public static bool ValidateObjectToSchema(object obj, JSchema schema)
        //{
        //    string json = JsonConvert.SerializeObject(obj);
        //    JObject deserializedObject = JObject.Parse(json);
        //    IList<string> errors;
        //    bool isValid = deserializedObject.IsValid(schema, out errors);
        //    if (!isValid)
        //    {
        //        Debug.LogWarning("Validation errors:");
        //        foreach (string error in errors)
        //        {
        //            Debug.LogWarning(error);
        //        }
        //    }
        //    return isValid;
        //}
    }
}
