using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class OriginalSceneController : IOriginalSceneController
    {
        IObjectRegistry registry;
        readonly ITexturesManager texturesManager;
        public OriginalSceneController(IObjectRegistry registry, ITexturesManager texturesManager)
        {
            this.registry = registry;
            this.texturesManager = texturesManager;
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
                    texturesManager.ApplyProtectedMaterialToObject(obj, originalMaterial);
                    objectInfo.materialWasRestored = true;
                }
            }
        }
    }
}
