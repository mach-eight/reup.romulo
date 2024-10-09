using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.Registry
{
    public class RegisteredIdentifierTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        IObjectRegistry objectRegistry;
        GameObject testObj0;
        GameObject testObj1;


        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            objectRegistry = sceneObjects.objectRegistry;
            yield return null;
        }

        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            Destroy(testObj0);
            objectRegistry.ClearRegistry();
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [UnityTest]
        public IEnumerator TestObjHasAnId()
        {
            testObj0 = new GameObject("testObj");
            testObj0.AddComponent<RegisteredIdentifier>();
            yield return null;
            var id = testObj0.GetComponent<RegisteredIdentifier>().getId();
            Assert.IsNotNull(id);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ObjectRegistryContainsTestObj()
        {
            testObj0 = new GameObject("testObj");
            testObj0.AddComponent<RegisteredIdentifier>();
            yield return null;
            var id = testObj0.GetComponent<RegisteredIdentifier>().getId();
            var obtainedObj = objectRegistry.GetObjectWithGuid(id);
            Assert.AreEqual(testObj0, obtainedObj);
            yield return null;
        }

        [UnityTest]
        public IEnumerator NoObjectIsReturnedIfIncorrectId()
        {
            var obtainedObj = objectRegistry.GetObjectWithGuid("an-incorrect-id");
            Assert.IsNull(obtainedObj);
            yield return null;
        }
        [UnityTest]
        public IEnumerator ObjectRegistryIsUpdatedIfNewIdIsAssigned()
        {
            int objectsInRegistryBefore = objectRegistry.GetObjectsCount();
            testObj0 = new GameObject("testObj");
            testObj0.AddComponent<RegisteredIdentifier>();
            yield return null;
            string currentId = testObj0.GetComponent<RegisteredIdentifier>().getId();
            Assert.AreEqual(testObj0, objectRegistry.GetObjectWithGuid(currentId));
            Assert.AreEqual(objectsInRegistryBefore + 1, objectRegistry.GetObjectsCount());
            string newId = "new-id";
            testObj0.GetComponent<RegisteredIdentifier>().AssignId(newId);
            yield return null;
            Assert.IsNull(objectRegistry.GetObjectWithGuid(currentId));
            Assert.AreEqual(testObj0, objectRegistry.GetObjectWithGuid(newId));
            Assert.AreEqual(objectsInRegistryBefore + 1, objectRegistry.GetObjectsCount());
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToGenerateIdRightAfterCreatingTheUniqueIdComponent()
        {
            GameObject gameObject = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = gameObject.AddComponent<RegisteredIdentifier>();
            yield return null;
            Assert.IsNotNull(registeredIdentifier.getId());
            Assert.AreEqual(gameObject, objectRegistry.GetObjectWithGuid(registeredIdentifier.getId()));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToAssignIdRightAfterCreatingTheUniqueIdComponent()
        {
            GameObject gameObject = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = gameObject.AddComponent<RegisteredIdentifier>();
            string assignedId = "assigned-id";
            registeredIdentifier.AssignId(assignedId);
            Assert.IsNotNull(registeredIdentifier.getId());
            Assert.AreEqual(gameObject, objectRegistry.GetObjectWithGuid(assignedId));
            yield return null;
        }

    }
}
