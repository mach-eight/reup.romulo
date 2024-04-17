using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using System.Collections.Generic;
using UnityEngine;

namespace Tests.PlayMode.Mocks
{
    public class RegistrySpy : IRegistry
    {
        public List<GameObject> objects = new List<GameObject>();
        public RegistrySpy()
        {
            for (int i = 0; i < 10; i++)
            {
                GameObject obj = new();
                obj.AddComponent<UniqueId>().GenerateId();
                objects.Add(obj);
            }
        }
        public void AddItem(GameObject item)
        {
            throw new System.NotImplementedException();
        }

        public void ClearRegistry()
        {
            throw new System.NotImplementedException();
        }

        public int GetItemCount()
        {
            throw new System.NotImplementedException();
        }

        public List<GameObject> GetItemsWithGuids(string[] guids)
        {
            return objects;
        }

        public GameObject GetItemWithGuid(string guid)
        {
            throw new System.NotImplementedException();
        }

        public void RemoveItem(GameObject item)
        {
            throw new System.NotImplementedException();
        }
    }
}
