using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class DataValidator
    {
        public static bool ValidateObjectToSchemaKeys(object obj, Dictionary<string, object> schemaKeys)
        {
            string json = JsonConvert.SerializeObject(obj);
            return ValidateJsonStringTooSchemaKeys(json, schemaKeys);
        }

        public static bool ValidateJsonStringTooSchemaKeys(string json, Dictionary<string, object> schemaKeys)
        {
            JObject jsonObj = JObject.Parse(json);

            foreach(KeyValuePair<string, object> pair in schemaKeys)
            {
                if (jsonObj[pair.Key] == null)
                {
                    Debug.LogWarning($"Key {pair.Key} not found in json object");
                    return false;
                }
                if (!ValidateObjectKeyType(jsonObj, pair.Key, pair.Value)){
                    return false;
                }
            }
            return true;
        }

        static private bool ValidateObjectKeyType(JObject obj, string key, object expectedValueType)
        {
                if (TypeIsNestedObject(expectedValueType))
                {
                    if (!ValidateJsonStringTooSchemaKeys(obj[key].ToString(), (Dictionary<string, object>)expectedValueType))
                    {
                        return false;
                    }
                }
                else if (obj[key].Type != (JTokenType)expectedValueType)
                {
                    Debug.LogWarning($"Key {key} is not of type {expectedValueType}");
                    return false;
                }
            return true;
        }

        static private bool TypeIsNestedObject(object type)
        {
            return type is Dictionary<string, object>;
        }

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
