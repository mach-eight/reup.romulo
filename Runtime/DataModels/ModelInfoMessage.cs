using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class ModelInfoMessage
    {
        public string buildVersion;
        public ObjectDTO building;
    }
}
