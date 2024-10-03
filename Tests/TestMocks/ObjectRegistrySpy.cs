using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using ReupVirtualTwin.controllers;

namespace ReupVirtualTwinTests.mocks
{
    public class ObjectRegistrySpy : IObjectRegistry
    {
        public List<GameObject> objects = new List<GameObject>();
        public string[] lastRequestedObjectIds;
        public List<string[]> requestedObjectIds;
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
            requestedObjectIds = new List<string[]>();
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
            return objects;
        }

        public GameObject GetObjectWithGuid(string guid)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveObject(string id, GameObject item)
        {
            throw new System.NotImplementedException();
        }
    }
}
