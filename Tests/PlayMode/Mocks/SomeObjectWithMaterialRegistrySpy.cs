using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.PlayMode.Mocks
{
    public class SomeObjectWithMaterialRegistrySpy : IObjectRegistry
    {
        public List<GameObject> objects = new List<GameObject>();
        public SomeObjectWithMaterialRegistrySpy()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = new();
                obj.AddComponent<UniqueId>().GenerateId();
                if (i % 2 == 0)
                {
                    obj.AddComponent<MeshRenderer>();
                }
                objects.Add(obj);
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

        public int GetObjectsCount()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetObjectsWithGuids(string[] guids)
        {
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
