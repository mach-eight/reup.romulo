using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class ChangeMaterialMessagePayload
    {
        public string material_url;
        public string[] object_ids;
    }
}
