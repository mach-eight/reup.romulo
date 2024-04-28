namespace ReupVirtualTwin.managers
{
    public interface MovementHaltDecitionMaker<T>
    {
        public bool ShouldKeepMoving(T target);
    }
}
