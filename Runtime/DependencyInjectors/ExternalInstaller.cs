using UnityEngine;
using Zenject;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ExternalInstaller : MonoInstaller
    {
        public GameObject building;
        public override void InstallBindings()
        {
            Container.Bind<GameObject>().WithId("building").FromInstance(building);
        }
    }
}
