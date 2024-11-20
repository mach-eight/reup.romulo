using UnityEngine;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.behaviours;
using UnityEngine.TestTools;
using System.Collections;
using ReupVirtualTwinTests.instantiators;

namespace ReupVirtualTwinTests.behaviours
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
            setupbuilding = sceneObjects.setupBuilding;
            building = sceneObjects.building;
            child0 = building.transform.GetChild(0).gameObject;
            grandChild0 = child0.transform.GetChild(0).gameObject;
        }
        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [Test]
        public void ShouldAssignIdsToAllObjectsInBuilding()
        {
            Assert.IsNotNull(building.GetComponent<RegisteredIdentifier>());
            Assert.IsNotNull(child0.GetComponent<RegisteredIdentifier>());
            Assert.IsNotNull(grandChild0.GetComponent<RegisteredIdentifier>());
        }

        [Test]
        public void ShouldAssignObjectInfoComponentToAllObjectsInBuilding()
        {
            Assert.IsNotNull(building.GetComponent<ObjectInfo>());
            Assert.IsNotNull(child0.GetComponent<ObjectInfo>());
            Assert.IsNotNull(grandChild0.GetComponent<ObjectInfo>());
        }

    }
}
