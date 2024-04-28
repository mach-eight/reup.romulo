using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;
using UnityEngine;

namespace ReupVirtualTwin
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
            heightMediator.colliderController = new CharacterColliderBoxController(character);
            heightMediator.initialSpawn = initialSpawnContainer.GetComponent<IInitialSpawn>();
            heightMediator.buildingLayerMask = buildingLayerMask;
        }
    }
}
