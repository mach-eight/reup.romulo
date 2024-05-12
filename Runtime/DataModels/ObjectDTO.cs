using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class ObjectDTO
    {
        public string id;
        public Tag[] tags;
        public ObjectDTO[] children;
    }
}
