using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using ReupVirtualTwin.controllers;
using System.Linq;

namespace ReupVirtualTwinTests.mocks
{
    public class ObjectRegistrySpy : IObjectRegistry
    {
        public List<GameObject> objects = new List<GameObject>();
        public string[] lastRequestedObjectIds = new string[] { };
        public List<string[]> requestedObjectIds = new List<string[]>();
        public bool returnDefaultObjects = false;
        private TagSystemController tagSystemController = new TagSystemController();
        public ObjectRegistrySpy()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = new();
                obj.AddComponent<UniqueId>().GenerateId();
                objects.Add(obj);
                tagSystemController.AssignTagSystemToObject(obj);
            }
            returnDefaultObjects = true;
        }
        public ObjectRegistrySpy(List<GameObject> objects)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                GameObject obj = objects[i];
                obj.AddComponent<UniqueId>().GenerateId();
                this.objects.Add(obj);
                tagSystemController.AssignTagSystemToObject(obj);
            }
        }
        public void AddObject(GameObject item)
        {
            throw new System.NotImplementedException();
        }

        public void ClearRegistry()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetObjects()
        {
            return objects;
        }

        public int GetObjectsCount()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetObjectsWithGuids(string[] guids)
        {
            lastRequestedObjectIds = guids;
            requestedObjectIds.Add(guids);

            if (returnDefaultObjects)
            {
                return objects;
            }

            List<GameObject> foundObjects = new List<GameObject>();
            foreach (string guid in guids)
            {
                GameObject obj = objects.FirstOrDefault(obj => obj.GetComponent<IUniqueIdentifier>().getId() == guid);
                foundObjects.Add(obj);
            }
            return foundObjects;
        }

        public GameObject GetObjectWithGuid(string guid)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveObject(string id, GameObject item)
        {
            throw new System.NotImplementedException();
        }

        public string[] GetObjectListIds()
        {
            return objects.ConvertAll(obj => obj.GetComponent<IUniqueIdentifier>().getId()).ToArray();
        }
    }
}
