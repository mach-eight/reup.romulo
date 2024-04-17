using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwin.models;
using UnityEditor;
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwinTests.Registry
{
    public class ObjectRegistryTests : MonoBehaviour
    {
        GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
        GameObject objectRegistryGameObject;
        IRegistry objectRegistry;
        GameObject testObj0;
        GameObject testObj1;

        [SetUp]
        public void SetUp()
        {
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
            objectRegistry = objectRegistryGameObject.GetComponent<IRegistry>();
        }

        [TearDown]
        public void TearDown()
        {
            Destroy(testObj0);
            Destroy(testObj1);
            objectRegistry.ClearRegistry();
            Destroy(objectRegistryGameObject);
        }

        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            yield return new WaitForSeconds(0.2f);
        }

        [UnityTest]
        public IEnumerator ShouldAddAnItemToRegistry()
        {
            testObj0 = new GameObject("testObj");
            IUniqueIdentifier uniqueIdentifier = testObj0.AddComponent<UniqueId>();
            string id = uniqueIdentifier.GenerateId();
            objectRegistry.AddItem(testObj0);
            var retrievedObj = objectRegistry.GetItemWithGuid(id);
            Assert.AreEqual(testObj0, retrievedObj);
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotAddAnItemWithNoIdentifierToRegistry()
        {
            testObj0 = new GameObject("testObj");
            Assert.That(() => objectRegistry.AddItem(testObj0), Throws.Exception);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotAddAnItemWithNoIdToRegistry()
        {
            testObj0 = new GameObject("testObj");
            testObj0.AddComponent<UniqueId>();
            Assert.That(() => objectRegistry.AddItem(testObj0), Throws.Exception);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldAddSeveralItemsToRegistry()
        {
            testObj0 = new GameObject("testObj0");
            IUniqueIdentifier uniqueIdentifier0 = testObj0.AddComponent<UniqueId>();
            string id0 = uniqueIdentifier0.GenerateId();
            objectRegistry.AddItem(testObj0);
            var retrievedObj0 = objectRegistry.GetItemWithGuid(id0);
            Assert.AreEqual(testObj0, retrievedObj0);
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            yield return null;
            testObj1 = new GameObject("testObj1");
            IUniqueIdentifier uniqueIdentifier1 = testObj1.AddComponent<UniqueId>();
            string id1 = uniqueIdentifier1.GenerateId();
            objectRegistry.AddItem(testObj1);
            var retrievedObj1 = objectRegistry.GetItemWithGuid(id1);
            Assert.AreEqual(testObj1, retrievedObj1);
            Assert.AreEqual(2, objectRegistry.GetItemCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRemoveItem()
        {
            testObj0 = new GameObject("testObj0");
            IUniqueIdentifier uniqueIdentifier0 = testObj0.AddComponent<UniqueId>();
            string id = uniqueIdentifier0.GenerateId();
            objectRegistry.AddItem(testObj0);
            var retrievedObj = objectRegistry.GetItemWithGuid(id);
            Assert.AreEqual(testObj0, retrievedObj);
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            yield return null;

            objectRegistry.RemoveItem(testObj0);
            Assert.AreEqual(0, objectRegistry.GetItemCount());
            Assert.IsNull(objectRegistry.GetItemWithGuid(id));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShouldClearRegistry()
        {
            testObj0 = new GameObject("testObj0");
            IUniqueIdentifier uniqueIdentifier0 = testObj0.AddComponent<UniqueId>();
            uniqueIdentifier0.GenerateId();
            objectRegistry.AddItem(testObj0);
            testObj1 = new GameObject("testObj1");
            IUniqueIdentifier uniqueIdentifier1 = testObj1.AddComponent<UniqueId>();
            uniqueIdentifier1.GenerateId();
            objectRegistry.AddItem(testObj1);
            Assert.AreEqual(2, objectRegistry.GetItemCount());
            yield return null;
            objectRegistry.ClearRegistry();
            Assert.AreEqual(0, objectRegistry.GetItemCount());
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShouldNotRaiseAnyExeptionIfAttemptToRemoveItemNotInRegistry()
        {
            testObj0 = new GameObject("testObj0");
            testObj0.AddComponent<UniqueId>().GenerateId();
            testObj1 = new GameObject("testObj1");
            objectRegistry.AddItem(testObj0);
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            yield return null;
            objectRegistry.RemoveItem(testObj1);
            Assert.AreEqual(1, objectRegistry.GetItemCount());
            yield return null;
        }

    }
}
