using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;
using UnityEngine;
using ReupVirtualTwin.behaviourInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class HeightMediatorDependecyInjector : MonoBehaviour
    {
        HeightMediator heightMediator;

        [SerializeField]
        GameObject maintainheightContainer;
        [SerializeField]
        GameObject initialSpawnContainer;
        [SerializeField]
        LayerMask buildingLayerMask;
        [SerializeField]
        GameObject character;

        void Awake()
        {
            heightMediator = GetComponent<HeightMediator>();
            heightMediator.maintainHeight = maintainheightContainer.GetComponent<IMaintainHeight>();
            heightMediator.initialSpawn = initialSpawnContainer.GetComponent<IInitialSpawn>();
            heightMediator.buildingLayerMask = buildingLayerMask;
        }
    }
}
