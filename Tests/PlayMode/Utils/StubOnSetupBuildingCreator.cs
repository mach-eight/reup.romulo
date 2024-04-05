using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.enums;
using System;
using UnityEngine;

public static class StubOnSetupBuildingCreator
{
    public static void CreateOnSetupBuilding()
    {
        GameObject setupBuildingGameObject = new("fake setup building");
        Debug.Log("just created setupBuildingGameObject " + setupBuildingGameObject.name);
        setupBuildingGameObject.tag = TagsEnum.setupBuilding;
        Debug.Log("just created setupBuildingGameObject tag " + setupBuildingGameObject.tag);
        setupBuildingGameObject.AddComponent<FakeSetupBuilding>();
    }
    private class FakeSetupBuilding : MonoBehaviour, IOnBuildingSetup
    {
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
