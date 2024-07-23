using ReupVirtualTwin.modelInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.models
{
    public class ObjectInfo : MonoBehaviour, IObjectInfo
    {
        public Material originalMaterial { get => _originalMaterial; }

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
