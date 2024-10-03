using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;
using System.Linq;

namespace ReupVirtualTwin.models
{
    public class ObjectRegistry : MonoBehaviour, IObjectRegistry
    {
        [HideInInspector] public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

        public void AddObject(GameObject item)
        {
            IUniqueIdentifier uniqueIdentifier = item.GetComponent<IUniqueIdentifier>();
            if (uniqueIdentifier == null || uniqueIdentifier.getId() == null)
            {
                throw new System.Exception("Object must have a unique identifier");
            }
            string guid = uniqueIdentifier.getId();
            if (objects.ContainsKey(guid))
            {
                throw new System.Exception($"An object with id '{guid}' already exists in registry, can't add object '{item.name}'");
            }
            objects.Add(guid, item);
        }
        public void RemoveObject(string id, GameObject item)
        {
            GameObject registeredItem = objects.GetValueOrDefault(id);
            if (registeredItem != item && registeredItem != null)
            {
                throw new System.Exception($"Object with id '{id}' is not the same as the object being removed '{item.name}'");
            }
            objects.Remove(id);
        }

        public GameObject GetObjectWithGuid(string guid)
        {
            return objects.GetValueOrDefault(guid);
        }
        public List<GameObject> GetObjectsWithGuids(string[] guids)
        {
            var foundObjects = new List<GameObject>();
            foreach (string guid in guids)
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

        public List<GameObject> GetObjects()
        {
            return objects.Values.ToList();
        }
    }
}
