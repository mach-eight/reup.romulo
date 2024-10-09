using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;

namespace ReupVirtualTwinTests.mocks
{
    public class TexturesManagerSpy : ITexturesManager
    {
        public List<GameObject> calledObjectsToApplyMaterial = new List<GameObject>();
        public List<Material> calledToApplyMaterials = new List<Material>();
        public List<GameObject> calledObjectsToApplyProtectedMaterial = new List<GameObject>();
        public List<Material> calledToApplyProtectedMaterials = new List<Material>();

        public void ApplyMaterialToObject(GameObject obj, Material material)
        {
            calledObjectsToApplyMaterial.Add(obj);
            calledToApplyMaterials.Add(material);
        }

        public void ApplyProtectedMaterialToObject(GameObject obj, Material material)
        {
            calledObjectsToApplyProtectedMaterial.Add(obj);
            calledToApplyProtectedMaterials.Add(material);
        }
    }

}