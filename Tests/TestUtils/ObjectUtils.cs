using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwinTests.utils
{
    public static class ObjectUtils
    {
        public static List<GameObject> FilterForObjectsWithMeshRenderer(List<GameObject> objects)
        {
            List<GameObject> filteredObjects = new List<GameObject>();
            for (int i = 0; i < objects.Count; i++)
            {
                if (objects[i].GetComponent<MeshRenderer>() != null)
                {
                    filteredObjects.Add(objects[i]);
                }
            }
            return filteredObjects;
        }

    }
}
