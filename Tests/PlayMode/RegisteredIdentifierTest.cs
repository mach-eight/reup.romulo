using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.models;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwinTests.Registry
{
    public class RegisteredIdentifierTest : MonoBehaviour
    {
        GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
        GameObject objectRegistryGameObject;
        IRegistry objectRegistry;
        GameObject testObj;

        [SetUp]
        public void SetUp()
        {
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
            objectRegistry = objectRegistryGameObject.GetComponent<IRegistry>();
            testObj = new GameObject("testObj");
            testObj.AddComponent<RegisteredIdentifier>();
        }
        [TearDown]
        public void TearDown()
        {
            Destroy(testObj);
            objectRegistry.ClearRegistry();
            Destroy(objectRegistryGameObject);
        }

        [UnityTest]
        public IEnumerator TestObjHasAnId()
        {
            yield return new WaitForSeconds(0.2f);
            var id = testObj.GetComponent<RegisteredIdentifier>().getId();
            Assert.IsNotNull(id);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ObjectRegistryContainsTestObj()
        {
            yield return new WaitForSeconds(0.2f);
            var id = testObj.GetComponent<RegisteredIdentifier>().getId();
            var obtainedObj = objectRegistry.GetItemWithGuid(id);
            Assert.AreEqual(testObj, obtainedObj);
            yield return null;
        }

        [UnityTest]
        public IEnumerator NoObjectIsReturnedIfIncorrectId()
        {
            yield return new WaitForSeconds(0.2f);
            var obtainedObj = objectRegistry.GetItemWithGuid("an-incorrect-id");
            Assert.IsNull(obtainedObj);
            yield return null;
        }
        [UnityTest]
        public IEnumerator ObjectIsRegistryIsUpdatedIfNewIdIsAssigned()
        {
            yield return new WaitForSeconds(0.2f);
            var currentId = testObj.GetComponent<RegisteredIdentifier>().getId();
            Assert.AreEqual(testObj, objectRegistry.GetItemWithGuid(currentId));
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            string newId = "new-id";
            testObj.GetComponent<RegisteredIdentifier>().AssignId(newId);
            yield return null;
            Assert.IsNull(objectRegistry.GetItemWithGuid(currentId));
            Assert.AreEqual(testObj, objectRegistry.GetItemWithGuid(newId));
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToGenerateIdRightAfterCreatingTheUniqueIdComponent()
        {
            yield return new WaitForSeconds(0.2f);
            GameObject gameObject = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = gameObject.AddComponent<RegisteredIdentifier>();
            registeredIdentifier.GenerateId();
            Assert.IsNotNull(registeredIdentifier.getId());
            Assert.AreEqual(gameObject, objectRegistry.GetItemWithGuid(registeredIdentifier.getId()));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToAssignIdRightAfterCreatingTheUniqueIdComponent()
        {
            yield return new WaitForSeconds(0.2f);
            GameObject gameObject = new GameObject("new-game-obj");
            RegisteredIdentifier registeredIdentifier = gameObject.AddComponent<RegisteredIdentifier>();
            string assignedId = "assigned-id";
            registeredIdentifier.AssignId(assignedId);
            Assert.IsNotNull(registeredIdentifier.getId());
            Assert.AreEqual(gameObject, objectRegistry.GetItemWithGuid(assignedId));
            yield return null;
        }
    }
}
