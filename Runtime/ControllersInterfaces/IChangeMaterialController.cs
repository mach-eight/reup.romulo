using ReupVirtualTwin.dataModels;
using System.Threading.Tasks;

namespace ReupVirtualTwin.controllerInterfaces
{
    public interface IChangeMaterialController
    {
        public Task ChangeObjectMaterial(ChangeMaterialMessagePayload message);
    }
}
