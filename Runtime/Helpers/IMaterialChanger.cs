using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public interface IMaterialChanger
    {
        public void SetNewMaterialToObjects(List<GameObject> objs, int[] submeshIndexes, Material material);
        public void SetNewMaterialToObject(GameObject obj, int submeshIndex, Material material);
    }
}
