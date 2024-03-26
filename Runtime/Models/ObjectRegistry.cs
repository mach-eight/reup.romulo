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
            objects.Add(item);
        }

        public GameObject GetItemWithGuid(string guid)
        {
            foreach (GameObject obj in objects)
            {
                if (obj == null) continue;
                var uniqueIdentifier = obj.GetComponent<IUniqueIdentifer>();
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

        public List<GameObject> GetItemTreesWithParentGuids(List<string> stringIDs)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            List<GameObject> allGameObjectsToEdit = new List<GameObject>();
            if (stringIDs != null && stringIDs.Count != 0)
            {
                gameObjects = GetItemsWithGuids(stringIDs.ToArray());
                gameObjects.RemoveAll(obj => obj == null);
                allGameObjectsToEdit.AddRange(gameObjects);
                foreach (GameObject obj in gameObjects)
                {
                    AddChildrenToList(obj.transform, allGameObjectsToEdit);
                }

            }
            return allGameObjectsToEdit;
        }
        private void AddChildrenToList(Transform parent, List<GameObject> list)
        {
            foreach (Transform childTransform in parent)
            {
                if (childTransform.gameObject != null)
                {
                    list.Add(childTransform.gameObject);
                    AddChildrenToList(childTransform, list);
                }
            }
        }

    }
}
