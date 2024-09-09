using UnityEngine;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwin.behaviours
{
    public class SetupBuildingTest
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        SetupBuilding setupbuilding;
        GameObject building;
        GameObject child0;
        GameObject grandChild0;

        [SetUp]
        public void SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            setupbuilding = sceneObjects.setupbuilding;
            building = sceneObjects.building;
            child0 = building.transform.GetChild(0).gameObject;
            grandChild0 = child0.transform.GetChild(0).gameObject;
        }
        [TearDown]
        public void TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        }

        [Test]
        public void ShouldAssignIdsToAllObjectsInBuilding()
        {
            Assert.IsNull(building.GetComponent<RegisteredIdentifier>());
            Assert.IsNull(child0.GetComponent<RegisteredIdentifier>());
            Assert.IsNull(grandChild0.GetComponent<RegisteredIdentifier>());
            setupbuilding.AssignIdsAndObjectInfoToBuilding();
            Assert.IsNotNull(building.GetComponent<RegisteredIdentifier>());
            Assert.IsNotNull(child0.GetComponent<RegisteredIdentifier>());
            Assert.IsNotNull(grandChild0.GetComponent<RegisteredIdentifier>());
        }

        [Test]
        public void ShouldAssignObjectInfoComponentToAllObjectsInBuilding()
        {
            Assert.IsNull(building.GetComponent<ObjectInfo>());
            Assert.IsNull(child0.GetComponent<ObjectInfo>());
            Assert.IsNull(grandChild0.GetComponent<ObjectInfo>());
            setupbuilding.AssignIdsAndObjectInfoToBuilding();
            Assert.IsNotNull(building.GetComponent<ObjectInfo>());
            Assert.IsNotNull(child0.GetComponent<ObjectInfo>());
            Assert.IsNotNull(grandChild0.GetComponent<ObjectInfo>());
        }

    }
}
