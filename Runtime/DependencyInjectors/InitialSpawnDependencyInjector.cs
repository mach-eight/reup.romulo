using UnityEngine;

using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class InitialSpawnDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject sensor;

        void Awake()
        {
            InitialSpawn initialSpawn = GetComponent<InitialSpawn>();
            IOnBuildingSetup setUpBuilding = ObjectFinder.FindSetupBuilding()?.GetComponent<IOnBuildingSetup>();
            if (setUpBuilding != null)
            {
                initialSpawn.setUpBuilding = setUpBuilding;
            }
            initialSpawn.sensor = sensor.GetComponent<IPointSensor>();
        }

    }
}