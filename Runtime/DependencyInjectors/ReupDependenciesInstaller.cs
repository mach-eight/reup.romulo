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
        public override void InstallBindings()
        {
            Container.Bind<string>().FromInstance("Hello World!");
            Container.Bind<Greeter>().AsSingle().NonLazy();

            Container.Bind<GameObject>().WithId("character").FromInstance(character);
            Container.Bind<ICharacterPositionManager>().To<CharacterPositionManager>().AsSingle();
        }
    }

    public class Greeter
    {
        public Greeter(string message)
        {
            Debug.Log(message);
        }
    }
}