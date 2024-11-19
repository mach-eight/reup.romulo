using System;

namespace ReupVirtualTwin.dataModels
{
    [Serializable]
    public class Tag
    {
        public int id;
        public string name;
        public string description;
        public int priority;

        public string str { get => $"{this.id} {this.name}"; }
    }
}
