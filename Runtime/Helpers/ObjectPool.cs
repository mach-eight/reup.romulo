using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

namespace ReupVirtualTwin.helpers
{
    public class ObjectPool : MonoBehaviour, IObjectPool
    {
        public List<GameObject> prefabsForPool;

        private List<GameObject> _pooledObjects = new List<GameObject>();

        public void AddPrefabType(GameObject prefabType)
        {
            prefabsForPool.Add(prefabType);
        }

        public GameObject GetObjectFromPool(string objectName, Transform parent = null)
        {
            // check if object with same name is already in the pool
            var instance = _pooledObjects.FirstOrDefault(obj => obj.name == objectName);
            if (instance != null)
            {
                _pooledObjects.Remove(instance);
                instance.SetActive(true);
                return instance;
            }

            // if object is not in the pool, create a new object
            if (parent == null)
            {
                throw new InvalidOperationException("object not in pool, and parent to instantiate new object not specified");
            }
            var prefab = prefabsForPool.FirstOrDefault(obj => obj.name == objectName);
            if (prefab != null)
            {
                var newInstance = Instantiate(prefab, parent);
                newInstance.name = objectName;
                return newInstance;
            }

            throw new InvalidOperationException("Object pool doesn't have a prefab for the object with name " + objectName);
        }

        public void PoolObject(GameObject obj)
        {
            obj.SetActive(false);
            _pooledObjects.Add(obj);
        }
    }
}
