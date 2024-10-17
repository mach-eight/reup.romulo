using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwinTests.utils;
using System.Text.RegularExpressions;

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
            GameObject.DestroyImmediate(testObj0);
            GameObject.DestroyImmediate(testObj1);
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
            testObj0 = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = testObj0.AddComponent<RegisteredIdentifier>();
            yield return null;
            Assert.IsNotNull(registeredIdentifier.getId());
            Assert.AreEqual(testObj0, objectRegistry.GetObjectWithGuid(registeredIdentifier.getId()));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToAssignIdRightAfterCreatingTheUniqueIdComponent()
        {
            testObj0 = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = testObj0.AddComponent<RegisteredIdentifier>();
            string assignedId = "assigned-id";
            registeredIdentifier.AssignId(assignedId);
            Assert.IsNotNull(registeredIdentifier.getId());
            Assert.AreEqual(testObj0, objectRegistry.GetObjectWithGuid(assignedId));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRaiseException_if_attemptToRegister2ObjectsWithSameManualId()
        {
            string repeatedId = "repeated-id";
            testObj0 = new GameObject("testObj0");
            RegisteredIdentifier registeredIdentifier0 = testObj0.AddComponent<RegisteredIdentifier>();
            registeredIdentifier0.manualId = repeatedId;
            yield return null;

            testObj1 = new GameObject("testObj1");
            RegisteredIdentifier registeredIdentifier1 = testObj1.AddComponent<RegisteredIdentifier>();
            registeredIdentifier1.manualId = repeatedId;
            LogAssert.Expect(LogType.Exception, new Regex($"An object with id '{repeatedId}' already exists in registry, can't add object '{testObj1.name}'"));
            yield return null;
        }

    }
}
