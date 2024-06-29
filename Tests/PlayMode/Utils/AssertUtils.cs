using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReupVirtualTwin.helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

}
