namespace ReupVirtualTwin.managerInterfaces
{
    public interface IDragManager
    {
        public bool primaryDragging { get; }
        public bool secondaryDragging { get; }
        public bool prevDragging { get; }
    }
}