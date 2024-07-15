using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
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
        public const string nullType = "null";

        private const string referredSchemaKey = "referredSchema";
        private const string schemaRefType = "$schemaRef";

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
        public static readonly JObject nullSchema = new JObject
        {
            { "type", nullType }
        };

        static public JObject MultiSchema(params JObject[] schemas)
        {
            return new JObject
            {
                { "oneOf", new JArray(schemas) }
            };
        }

        static public JObject CreateRefSchema(string refName)
        {
            return new JObject
            {
                { "type", schemaRefType },
                { referredSchemaKey, refName }
            };
        }

        static public JObject CreateArraySchema(params JObject[] itemSchemas)
        {
            return new JObject
            {
                { "type", arrayType },
                { "items", new JArray ( itemSchemas ) }
            };
        }

        static public bool ValidateObjectToSchema(object obj, JObject schema)
        {
            string json = JsonConvert.SerializeObject(obj);
            return ValidateJsonStringToSchema(json, schema);
        }

        static public bool ValidateJsonStringToSchema(string json, JObject schema)
        {
            JToken jsonObj = JsonConvert.DeserializeObject<JToken>(json);
            return ValidateJTokenToSchema(jsonObj, schema);
        }

        static bool ValidateJTokenToSchema(JToken obj, JObject schema)
        {
            return ValidateJTokenToSchema(obj, schema, null);
        }

        static bool ValidateJTokenToSchema(JToken obj, JObject schema, string key)
        {
            if (schema == null)
            {
                return false;
            }
            if (schema["oneOf"] != null)
            {
                return ValidateJTokenToAnySchema(obj, (JArray)schema["oneOf"], key);
            }
            switch ((string)schema["type"])
            {
                case boolType:
                    return ValidateJObjectType(obj, JTokenType.Boolean, key);
                case intType:
                    return ValidateJObjectType(obj, JTokenType.Integer, key);
                case stringType:
                    return ValidateJObjectType(obj, JTokenType.String, key);
                case objectType:
                    return ValidateJObjectProperties(obj, schema);
                case arrayType:
                    return ValidateJArrayItems(obj, (JArray)schema["items"]);
                case schemaRefType:
                    return ValidateObjectToSchemaRef(obj, schema);
                case nullType:
                    return ValidateToNull(obj);
                default:
                    Debug.LogWarning($"Type {schema["type"]} not supported");
                    return false;
            }
        }
        static bool ValidateToNull(JToken obj)
        {
            return obj.Type == JTokenType.Null;
        }
        static bool ValidateObjectToSchemaRef(JToken obj, JObject schemaRef)
        {
            JObject referredSchema = GetReferredSchemaFromInnerSchema(schemaRef, (string)schemaRef[referredSchemaKey]);
            return ValidateJTokenToSchema(obj, referredSchema);
        }
        static JObject GetReferredSchemaFromInnerSchema(JContainer innerSchema, string refSchemaName)
        {
            if (innerSchema == null)
            {
                Debug.LogWarning($"Could not find reference to {refSchemaName}");
                return null;
            }
            JContainer parentObj = innerSchema.Parent;
            if (IsJObjectReferredSchema(parentObj, refSchemaName))
            {
                return (JObject)parentObj;
            }
            return GetReferredSchemaFromInnerSchema(parentObj, refSchemaName);
        }
        static bool IsJObjectReferredSchema(JContainer container, string refSchemaName)
        {
            return container != null &&
                container is JObject &&
                container["name"] != null &&
                container["name"].ToString() == refSchemaName;
        }
        static bool ValidateJArrayItems(JToken array, JArray acceptedSchemas)
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
        static bool ValidateJTokenToAnySchema(JToken obj, JArray schemas, string key)
        {
            bool result = ValidateJTokenToAnySchema(obj, schemas);
            if (key != null && result == false)
            {
                string schemasAsList = string.Join(", ", schemas.ToList().Select(schema => schema["type"].ToString()));
                Debug.LogWarning($"Key {key} is not of any of the accepted schemas: {schemasAsList}");
            }
            return result;
        }
        static bool ValidateJTokenToAnySchema(JToken obj, JArray schemas)
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
        static bool ValidateJObjectProperties(JToken obj, JObject schema)
        {
            JObject properties = (JObject)schema["properties"];
            JArray required = (JArray)schema["required"];
            if (!ValidateJObjectType(obj, JTokenType.Object))
            {
                return false;
            }
            foreach (KeyValuePair<string, JToken> pair in properties)
            {
                if (obj[pair.Key] == null && IsItemInArray<string>(required, pair.Key))
                {
                    Debug.LogWarning($"Key {pair.Key} not found in object");
                    return false;
                }
                if (obj[pair.Key] != null && !ValidateJTokenToSchema(obj[pair.Key], (JObject)pair.Value, pair.Key))
                {
                    Debug.LogWarning($"Validation of {pair.Key} value failed");
                    return false;
                }
            }
            return true;
        }

        static bool ValidateJObjectType(JToken obj, JTokenType expectedType, string key)
        {
            bool valid = ValidateJObjectType(obj, expectedType);
            if (!valid && !string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"Unexpected {obj.Type} value in {key} key, expected to be of type {expectedType}");
            }
            return valid;
        }

        static bool ValidateJObjectType(JToken obj, JTokenType expectedType)
        {
            if (obj.Type != expectedType)
            {
                return false;
            }
            return true;
        }

        static bool IsItemInArray<T>(JArray array, T item)
        {
            if (array == null)
            {
                return false;
            }
            return array.Any(arrayItem => arrayItem.Value<T>().Equals(item));
        }

    }
}
