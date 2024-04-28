
namespace ReupVirtualTwin.modelInterfaces
{
    public interface IUniqueIdentifier
    {
        public string getId();
        public bool isIdCorrect(string id);
        public string GenerateId();
        public string AssignId(string id);
    }
}
