using System.Collections;
using System.Collections.Generic;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class TexturesManager : MonoBehaviour, ITexturesManager
    {
        public void ApplyMaterialToObject(GameObject obj, Material material)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            Texture oldTexture = renderer.material.GetTexture("_BaseMap");
            renderer.material = material;
            Destroy(oldTexture);
        }
    }

}