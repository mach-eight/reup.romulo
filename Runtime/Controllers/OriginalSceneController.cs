using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class OriginalSceneController : IOriginalSceneController
    {
        IObjectRegistry registry;
        public OriginalSceneController(IObjectRegistry registry)
        {
            this.registry = registry;
        }
        public void RestoreOriginalScene()
        {
            List<GameObject> allObjects = registry.GetObjects();
            foreach (GameObject obj in allObjects)
            {
                IObjectInfo objectInfo = obj.GetComponent<IObjectInfo>();
                Material originalMaterial = objectInfo.originalMaterial;
                MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer>();
                if (meshRenderer != null && objectInfo.materialWasChanged)
                {
                    meshRenderer.material = originalMaterial;
                    objectInfo.materialWasRestored = true;
                }
            }
        }
    }
}
