using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface IObjectPool
    {
        void AddPrefabType(GameObject prefabType);
        GameObject GetObjectFromPool(string objectName, Transform parent = null);
        void PoolObject(GameObject obj);
    }
}