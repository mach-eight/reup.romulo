using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class ObjectInfoController : IObjectInfoController
    {
        public void AssignObjectInfoToTree(GameObject tree, string parentTreeId = null)
        {
            AssignObjectInfoToObject(tree);
            foreach (Transform child in tree.transform)
            {
                AssignObjectInfoToTree(child.gameObject);
            }
        }
        public IObjectInfo AssignObjectInfoToObject(GameObject obj)
        {
            IObjectInfo objectInfo = obj.GetComponent<IObjectInfo>();
            if (objectInfo == null)
            {
                objectInfo = obj.AddComponent<ObjectInfo>();
            }
            return objectInfo;
        }
    }
}
