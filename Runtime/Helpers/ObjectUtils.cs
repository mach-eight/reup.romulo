using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
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

        public static void ActivateAllObjects(GameObject obj)
        {
            obj.SetActive(true);
            foreach (Transform child in obj.transform)
            {
                ActivateAllObjects(child.gameObject);
            }
        }

    }
}
