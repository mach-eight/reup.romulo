using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IChangeColorManager
    {
        public List<GameObject> GetObjectsToChangeColor(List<string> stringIDs);
        public void ChangeObjectsColor(List<GameObject> objectsToDelete, Color color);
    }
}

