using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.modelInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ReupVirtualTwinTests.utils
{
    public static class AssertUtils
    {

        public static void AssertAllObjectsWithMeshRendererHaveMetaDataValue<T>(List<GameObject> objects, string metaDataPath, object metaDataValue)
        {
            List<JToken> metaDataValues = ObjectMetaDataUtils.GetMetaDataValuesFromObjects(objects, metaDataPath);
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<MeshRenderer>() != null)
                {
                    Assert.AreEqual(metaDataValue, metaDataValues[i].ToObject<T>());
                }
                else
                {
                    Assert.IsNull(metaDataValues[i]);
                }
            }
        }

        public static void AssertAllAreNull<T>(List<T> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                Assert.IsNull(list[i]);
            }
        }

        public static void AssertAllObjectsWithMeshRendererHaveSetChangedMaterial(List<GameObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<MeshRenderer>() != null)
                {
                    Assert.IsTrue(objects[i].GetComponent<IObjectInfo>().materialWasChanged);
                }
            }
        }

        public static void AssertVector2sAreEqual(Vector2 expected, Vector2 real)
        {
            float distance = Vector2.Distance(expected, real);
            Assert.IsTrue(distance < 1e-6);
        }


    }
}
