using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.models
{
    public class ObjectRegistry : MonoBehaviour, IRegistry
    {
        [HideInInspector]
        public List<GameObject> objects = new List<GameObject>();

        public void AddItem(GameObject item)
        {
            IUniqueIdentifier uniqueIdentifier = item.GetComponent<IUniqueIdentifier>();
            if (uniqueIdentifier == null || uniqueIdentifier.getId() == null)
            {
                throw new System.Exception("Object must have a unique identifier");
            }
            objects.Add(item);
        }
        public void RemoveItem(GameObject item)
        {
            objects.Remove(item);
        }

        public GameObject GetItemWithGuid(string guid)
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
        public List<GameObject> GetItemsWithGuids(string[] guids)
        {
            var foundObjects = new List<GameObject>();
            foreach(string  guid in guids)
            {
                foundObjects.Add(GetItemWithGuid(guid));
            }
            return foundObjects;
        }

        public int GetItemCount()
        {
            return objects.Count;
        }
        public void ClearRegistry()
        {
            objects.Clear();
        }
    }
}
