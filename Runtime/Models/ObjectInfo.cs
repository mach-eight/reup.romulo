using ReupVirtualTwin.modelInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.models
{
    public class ObjectInfo : MonoBehaviour, IObjectInfo, IObjectMetaDataGetterSetter
    {
        public JObject objectMetaData { get => _objectMetaData; set => _objectMetaData = value; }
        public Material originalMaterial { get => _originalMaterial; }
        public bool materialWasRestored { get; set; } = false;
        public bool materialWasChanged { get; set; } = false;

        private JObject _objectMetaData;

        private Material _originalMaterial;

        void Start()
        {
            RegisterOriginalMaterial();
        }

        private void RegisterOriginalMaterial()
        {
            MeshRenderer renderer = GetComponent<MeshRenderer>();
            if (renderer != null && renderer.sharedMaterial != null)
            {
                _originalMaterial = renderer.sharedMaterial;
            }
        }

    }
}
