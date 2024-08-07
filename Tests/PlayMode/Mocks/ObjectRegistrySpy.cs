using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.PlayMode.Mocks
{
    public class ObjectRegistrySpy : IObjectRegistry
    {
        public List<GameObject> objects = new List<GameObject>();
        public string[] lastRequestedObjectIds;
        public List<string[]> requestedObjectIds;
        public ObjectRegistrySpy()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = new();
                obj.AddComponent<UniqueId>().GenerateId();
                objects.Add(obj);
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

        public void RemoveObject(GameObject item)
        {
            throw new System.NotImplementedException();
        }
    }
}
