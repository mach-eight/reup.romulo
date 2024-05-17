using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class DataValidator
    {
        public static bool ValidateObjectToSchema(object obj, JSchema schema)
        {
            string json = JsonConvert.SerializeObject(obj);
            JObject deserializedObject = JObject.Parse(json);
            IList<string> errors;
            bool isValid = deserializedObject.IsValid(schema, out errors);
            if (!isValid)
            {
                Debug.LogWarning("Validation errors:");
                foreach (string error in errors)
                {
                    Debug.LogWarning(error);
                }
            }
            return isValid;
        }
    }
}
