namespace ReupVirtualTwin.managerInterfaces
{
    public interface IDragManager
    {
        public bool dragging { get; }
        public bool prevDragging { get; }
        public bool selectInputInUI { get; }
        public bool prevSelectInputInUI { get; }
    }
}