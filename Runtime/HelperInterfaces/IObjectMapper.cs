using UnityEngine;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;

namespace ReupVirtualTwin.helperInterfaces
{
    public interface IObjectMapper
    {
        public ObjectDTO MapObjectToDTO(GameObject obj);
        public ObjectDTO[] MapObjectsToDTO(List<GameObject> objs);
        public ObjectDTO MapObjectTree(GameObject obj);
    }
}
