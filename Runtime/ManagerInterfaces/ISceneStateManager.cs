using Newtonsoft.Json.Linq;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface ISceneStateManager
    {
        public JObject GetSceneState();
    }
}
