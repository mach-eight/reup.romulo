using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class StartupMessage
    {
        public string buildVersion;
        public ObjectDTO building;
    }
}
