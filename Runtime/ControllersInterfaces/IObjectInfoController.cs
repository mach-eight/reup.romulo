using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IObjectInfoController
    {
        public void AssignObjectInfoToTree(GameObject tree, string parentTreeId = null);
    }
}
