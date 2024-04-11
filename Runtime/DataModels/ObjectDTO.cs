using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class ObjectDTO
    {
        public string id;
        public string[] tags;
        public ObjectDTO[] children;
    }
}
