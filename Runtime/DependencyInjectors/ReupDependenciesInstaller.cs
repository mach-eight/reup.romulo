using Zenject;
using UnityEngine;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.inputs;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class ReupDependenciesInstaller : MonoInstaller
    {
        public GameObject character;
        public GameObject innerCharacter;
        public Transform dhvWrapper;
        public DiContainer container;
        public override void InstallBindings()
        {
            container = Container;
            Container.BindInterfacesAndSelfTo<InputProvider>().AsSingle();
            Container.Bind<GameObject>().WithId("character").FromInstance(character);
            Container.Bind<GameObject>().WithId("innerCharacter").FromInstance(innerCharacter);
            Container.Bind<Transform>().WithId("dhvWrapper").FromInstance(dhvWrapper);
            Container.BindInterfacesAndSelfTo<CharacterPositionManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<CharacterRotationManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<DragManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<GesturesManager>().AsSingle();
            Container.Bind<ITagsController>().To<TagsController>().AsSingle();
            Container.Bind<int>().WithId("buildingLayerId").FromInstance(6);
            Container.BindInterfacesAndSelfTo<ZoomPositionRotationDHVController>().AsSingle();
        }
    }
}
