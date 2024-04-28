using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IDeleteObjectsManager
    {
        public void DeleteObjects(List<GameObject> objectsToDelete);
        public List<GameObject> GetDeletableObjects(string stringIDs);
    }
}