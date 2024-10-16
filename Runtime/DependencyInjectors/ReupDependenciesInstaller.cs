using Zenject;
using UnityEngine;
using System.Collections;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;

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
        }
    }
}
