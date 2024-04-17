using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface IObjectRegistry
    {
        public void AddObject(GameObject item);
        public void RemoveObject(GameObject item);
        public GameObject GetObjectWithGuid(string guid);
        public List<GameObject> GetObjectsWithGuids(string[] guids);
        public int GetObjectsCount();
        public void ClearRegistry();
    }
}
