using Newtonsoft.Json.Linq;
using ReupVirtualTwin.modelInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class ObjectMetaDataUtils
    {
        static readonly string materialIdPath = "appearance.materialId";
        static readonly string colorPath = "appearance.color";
        public static JObject GetMetaData(GameObject gameObject)
        {
            if (gameObject.TryGetComponent(out IObjectMetaDataGetterSetter objectMetaDataComponent))
            {
                return objectMetaDataComponent.objectMetaData;
            }
            return null;
        }

        public static void AssignMetaDataToObject(GameObject gameObject, JObject objectMetaData)
        {
            if (gameObject.TryGetComponent(out IObjectMetaDataGetterSetter objectMetaDataComponent))
            {
                objectMetaDataComponent.objectMetaData = objectMetaData;
                return;
            }
            throw new System.Exception($"Object {gameObject.name} does not have a IObjectMetaDataGetterSetter component");
        }

        public static JToken GetMetaDataValueFromObject(GameObject gameObject, string metaDataPath)
        {
            JObject objectMetaData = GetMetaData(gameObject);
            if (objectMetaData != null)
            {
                JToken valueToken = objectMetaData.SelectToken(metaDataPath);
                return valueToken;

            }
            return null;
        }

        public static JObject AssignMaterialIdMetaDataToObject(GameObject gameObject, int materialId)
        {
            RemoveValueFromMetaDataInObject(gameObject, colorPath);
            return AssignMetaDataValueInPathToObject(gameObject, materialIdPath, materialId);
        }

        public static JObject AssignColorMetaDataToObject(GameObject gameObject, string rgbaColor)
        {
            RemoveValueFromMetaDataInObject(gameObject, materialIdPath);
            return AssignMetaDataValueInPathToObject(gameObject, colorPath, rgbaColor);
        }
        public static JObject RemoveValueFromMetaDataInObject(GameObject gameObject, string metaDataPath)
        {
            JObject objectMetaData = GetMetaData(gameObject);
            if (objectMetaData != null)
            {
                JObjectUtils.RemoveValueFromPath(objectMetaData, metaDataPath);
                return objectMetaData;
            }
            return null;
        }
        public static JObject AssignMetaDataValueInPathToObject(GameObject gameObject, string path, object value)
        {
            JObject objectMetaData = GetMetaData(gameObject);
            if (objectMetaData != null)
            {
                JObjectUtils.SetValueToPath(objectMetaData, path, value);
                return objectMetaData;
            }
            JObject newObjectMetaData = new JObject();
            JObjectUtils.SetValueToPath(newObjectMetaData, path, value);
            AssignMetaDataToObject(gameObject, newObjectMetaData);
            return newObjectMetaData;
        }

        public static List<JToken> GetMetaDataValuesFromObjects(List<GameObject> objects, string metaDataPath)
        {
            List<JToken> metaData = new();
            for (int i = 0; i < objects.Count; i++)
            {
                metaData.Add(GetMetaDataValueFromObject(objects[i], metaDataPath));
            }
            return metaData;
        }
    }
}
