using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.modelInterfaces
{
    public interface IRegistry
    {
        public void AddItem(GameObject item);
        public void RemoveItem(GameObject item);
        public GameObject GetItemWithGuid(string guid);
        public List<GameObject> GetItemsWithGuids(string[] guids);
        public List<GameObject> GetItemTreesWithParentGuids(List<string> stringIDs);
        public int GetItemCount();
        public void ClearRegistry();
    }
}
