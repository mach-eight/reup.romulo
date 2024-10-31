using System.Collections.Generic;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.models;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class TexturesManager : MonoBehaviour, ITexturesManager
    {
        public IIdGetterController idGetterController { set; get; }
        IObjectTexturesRecord objectsTexturesRecord;

        private void Awake()
        {
            objectsTexturesRecord = new ObjectTexturesRecord();
        }

        public void ApplyMaterialToObject(GameObject obj, Material material)
        {
            ApplyMaterial(obj, material);
            objectsTexturesRecord.UpdateRecords(obj);
        }

        public void ApplyProtectedMaterialToObject(GameObject obj, Material material)
        {
            ApplyMaterial(obj, material);
            objectsTexturesRecord.CleanRecords(obj);
        }

        void ApplyMaterial(GameObject obj, Material material)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            renderer.material = material;
        }
    }

}