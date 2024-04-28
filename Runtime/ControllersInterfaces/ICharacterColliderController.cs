namespace ReupVirtualTwin.controllerInterfaces
{
    public interface ICharacterColliderController
    {
        public bool UpdateCollider(float characterHeight);
        public void DestroyCollider();
    }
}
