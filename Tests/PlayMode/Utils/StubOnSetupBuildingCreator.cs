using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.enums;
using System;
using UnityEngine;

public static class StubOnSetupBuildingCreator
{
    public static GameObject CreateImmediateOnSetupBuilding(bool enabled = true)
    {
        GameObject setupBuildingGameObject = new("fake setup building");
        setupBuildingGameObject.SetActive(enabled);
        setupBuildingGameObject.tag = TagsEnum.setupBuilding;
        setupBuildingGameObject.AddComponent<FakeSetupBuilding>();
        return setupBuildingGameObject;
    }
    public class FakeSetupBuilding : MonoBehaviour, IOnBuildingSetup, IBuildingGetter
    {
        GameObject _building;
        public GameObject building { get => _building; set => _building = value; }

        event Action _onBuildingSetUp;
        public event Action onBuildingSetUp
        {
            add
            {
                _onBuildingSetUp += value;
            }
            remove { _onBuildingSetUp -= value; }
        }
        private void Start()
        {
            _onBuildingSetUp?.Invoke();
        }
    }
}
