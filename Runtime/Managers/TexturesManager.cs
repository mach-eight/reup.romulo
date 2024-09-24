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
            obj.GetComponent<Renderer>().material = material;
        }
    }

}