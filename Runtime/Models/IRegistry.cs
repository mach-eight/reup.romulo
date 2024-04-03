using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.models
{
    public interface IRegistry
    {
        public void AddItem(GameObject item);
        public void RemoveItem(GameObject item);
        public GameObject GetItemWithGuid(string guid);
        public List<GameObject> GetItemsWithGuids(string[] guids);
        public int GetItemCount();
        public void ClearRegistry();
    }
}
