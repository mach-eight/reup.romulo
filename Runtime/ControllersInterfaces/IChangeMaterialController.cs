using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IChangeMaterialController
    {
        public Task<TaskResult> ChangeObjectMaterial(JObject message);
    }

}
