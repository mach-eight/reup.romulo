using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class InitialSpawnDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject sensor;
        [SerializeField]
        GameObject characterPositionManager;

        void Awake() {
            InitialSpawn initialSpawn = GetComponent<InitialSpawn>();
            IOnBuildingSetup setUpBuilding = ObjectFinder.FindSetupBuilding()?.GetComponent<IOnBuildingSetup>();
            initialSpawn.characterPositionManager = characterPositionManager.GetComponent<ICharacterPositionManager>();
            if (setUpBuilding != null)
            {
                initialSpawn.setUpBuilding = setUpBuilding;
            }
            initialSpawn.sensor = sensor.GetComponent<ISensor>();
        }

    }
}