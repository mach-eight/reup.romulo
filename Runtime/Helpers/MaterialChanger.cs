using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class MaterialChanger : MonoBehaviour, IMaterialChanger
    {
        public void SetNewMaterialToObjects(List<GameObject> objs, int[] submeshIndexes, Material material)
        {
            foreach (var (obj, i) in objs.Select((v, i) => (v, i)))
            {
                SetNewMaterialToObject(obj, submeshIndexes[i], material);
            }
        }
        public void SetNewMaterialToObject(GameObject obj, int submeshIndex, Material material)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                throw new Exception($"Object {obj.name} has not renderer attached");
            }
            var materials = obj.GetComponent<Renderer>().materials;
            Material[] newMaterials = new Material[materials.Length];
            newMaterials = materials;
            newMaterials[submeshIndex] = material;
            obj.GetComponent<Renderer>().materials = newMaterials;
        }

    }
}
