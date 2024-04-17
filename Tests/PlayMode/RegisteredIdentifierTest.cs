using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.Registry
{
    public class RegisteredIdentifierTest : MonoBehaviour
    {
        GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
        GameObject objectRegistryGameObject;
        IObjectRegistry objectRegistry;
        GameObject testObj;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // we set the objectRegistry only once because some objects that depend on it are use the ObjectFinder class to find it
            // if we create a different objectRegistry for each test in the SetUp method, the ObjectFinder sometimes finds
            // an old objectRegistry why this happens is still unknown to me
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
            objectRegistry = objectRegistryGameObject.GetComponent<IObjectRegistry>();
        }
        [SetUp]
        public void SetUp()
        {
            testObj = new GameObject("testObj");
            testObj.AddComponent<RegisteredIdentifier>();
        }
        [TearDown]
        public void TearDown()
        {
            Destroy(testObj);
            objectRegistry.ClearRegistry();
        }
        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Destroy(objectRegistryGameObject);
        }

        [UnityTest]
        public IEnumerator TestObjHasAnId()
        {
            var id = testObj.GetComponent<RegisteredIdentifier>().getId();
            Assert.IsNotNull(id);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ObjectRegistryContainsTestObj()
        {
            var id = testObj.GetComponent<RegisteredIdentifier>().getId();
            var obtainedObj = objectRegistry.GetObjectWithGuid(id);
            Assert.AreEqual(testObj, obtainedObj);
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
        public IEnumerator ObjectIsRegistryIsUpdatedIfNewIdIsAssigned()
        {
            string currentId = testObj.GetComponent<RegisteredIdentifier>().getId();
            Assert.AreEqual(testObj, objectRegistry.GetObjectWithGuid(currentId));
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            string newId = "new-id";
            testObj.GetComponent<RegisteredIdentifier>().AssignId(newId);
            yield return null;
            Assert.IsNull(objectRegistry.GetObjectWithGuid(currentId));
            Assert.AreEqual(testObj, objectRegistry.GetObjectWithGuid(newId));
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToGenerateIdRightAfterCreatingTheUniqueIdComponent()
        {
            GameObject gameObject = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = gameObject.AddComponent<RegisteredIdentifier>();
            registeredIdentifier.GenerateId();
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
