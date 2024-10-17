using Zenject;
using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.inputs;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ReupDependenciesInstaller : MonoInstaller
    {
        public GameObject character;
        public DiContainer container;
        public override void InstallBindings()
        {
            container = Container;
            Container.Bind<GameObject>().WithId("character").FromInstance(character);
            Container.Bind<ICharacterPositionManager>().To<CharacterPositionManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<DragManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GesturesManager>().AsSingle();
        }
    }
}