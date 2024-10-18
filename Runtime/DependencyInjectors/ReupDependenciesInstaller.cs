using Zenject;
using UnityEngine;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.inputs;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ReupDependenciesInstaller : MonoInstaller
    {
        public GameObject character;
        public GameObject innerCharacter;
        public DiContainer container;
        public override void InstallBindings()
        {
            container = Container;
            Container.Bind<GameObject>().WithId("character").FromInstance(character);
            Container.Bind<GameObject>().WithId("innerCharacter").FromInstance(innerCharacter);
            Container.BindInterfacesAndSelfTo<CharacterPositionManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterRotationManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<InputProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<DragManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GesturesManager>().AsSingle();
        }
    }
}