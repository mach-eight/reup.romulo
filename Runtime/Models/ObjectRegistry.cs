using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;
using System.Linq;

namespace ReupVirtualTwin.models
{
    public class ObjectRegistry : MonoBehaviour, IObjectRegistry
    {
        [HideInInspector]
        public Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();

        public void AddObject(GameObject item)
        {
            IUniqueIdentifier uniqueIdentifier = item.GetComponent<IUniqueIdentifier>();
            if (uniqueIdentifier == null || uniqueIdentifier.getId() == null)
            {
                throw new System.Exception("Object must have a unique identifier");
            }
            string guid = uniqueIdentifier.getId();
            objects.Add(guid, item);
        }
        public void RemoveObject(GameObject item)
        {
            objects.Remove(item.GetComponent<IUniqueIdentifier>().getId());
        }

        public GameObject GetObjectWithGuid(string guid)
        {
            return objects[guid];
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
