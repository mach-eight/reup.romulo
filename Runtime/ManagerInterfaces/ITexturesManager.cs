using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ITexturesManager
    {
        public void ApplyMaterialToObject(GameObject obj, Material material);
    }
}