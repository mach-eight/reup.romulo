namespace ReupVirtualTwin.managerInterfaces
{
    public interface ICharacterRotationManager
    {
        public float ANGLE_THRESHOLD { get; }
        public bool allowRotation { set; get; }
        public float verticalRotation { set; get; }
        public float horizontalRotation { set; get; }
    }
}
