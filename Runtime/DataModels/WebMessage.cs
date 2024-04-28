using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class WebMessage<T>
    {
        public string type;
# nullable enable
        public T? payload;
# nullable disable
    }
}
