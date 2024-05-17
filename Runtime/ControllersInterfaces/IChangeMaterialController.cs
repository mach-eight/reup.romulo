using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IChangeMaterialController
    {
        public Task ChangeObjectMaterial(JObject message);
    }
}
