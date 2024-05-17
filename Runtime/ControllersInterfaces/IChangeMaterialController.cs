using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IChangeMaterialController
    {
        public Task ChangeObjectMaterial(Dictionary<string, object> message);
    }
}
