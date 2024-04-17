using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
    public class ObjectRegistry : MonoBehaviour, IObjectRegistry
    {
        [HideInInspector]
        public List<GameObject> objects = new List<GameObject>();

        public void AddObject(GameObject item)
        {
            IUniqueIdentifier uniqueIdentifier = item.GetComponent<IUniqueIdentifier>();
            if (uniqueIdentifier == null || uniqueIdentifier.getId() == null)
            {
                throw new System.Exception("Object must have a unique identifier");
            }
            objects.Add(item);
        }
        public void RemoveObject(GameObject item)
        {
            objects.Remove(item);
        }

        public GameObject GetObjectWithGuid(string guid)
        {
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                var uniqueIdentifier = obj.GetComponent<IUniqueIdentifier>();
                if (uniqueIdentifier.isIdCorrect(guid))
                {
                    return obj;
                }
            }
            return null;
        }
        public List<GameObject> GetObjectsWithGuids(string[] guids)
        {
            var foundObjects = new List<GameObject>();
            foreach(string  guid in guids)
            {
                foundObjects.Add(GetObjectWithGuid(guid));
            }
            return foundObjects;
        }

        public int GetObjectsCount()
        {
            return objects.Count;
        }
        public void ClearRegistry()
        {
            objects.Clear();
        }
    }
}
