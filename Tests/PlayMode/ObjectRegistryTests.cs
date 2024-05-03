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
        IObjectRegistry objectRegistry;
        GameObject testObj0;
        GameObject testObj1;

        [SetUp]
        public void SetUp()
        {
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
            objectRegistry = objectRegistryGameObject.GetComponent<IObjectRegistry>();
        }

        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            Destroy(testObj0);
            Destroy(testObj1);
            objectRegistry.ClearRegistry();
            Destroy(objectRegistryGameObject);
            yield return new WaitForSeconds(0.2f);
        }

        [UnityTest]
        public IEnumerator ShouldAddAnItemToRegistry()
        {
            testObj0 = new GameObject("testObj");
            IUniqueIdentifier uniqueIdentifier = testObj0.AddComponent<UniqueId>();
            string id = uniqueIdentifier.GenerateId();
            objectRegistry.AddObject(testObj0);
            var retrievedObj = objectRegistry.GetObjectWithGuid(id);
            Assert.AreEqual(testObj0, retrievedObj);
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotAddAnItemWithNoIdentifierToRegistry()
        {
            testObj0 = new GameObject("testObj");
            Assert.That(() => objectRegistry.AddObject(testObj0), Throws.Exception);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotAddAnItemWithNoIdToRegistry()
        {
            testObj0 = new GameObject("testObj");
            testObj0.AddComponent<UniqueId>();
            Assert.That(() => objectRegistry.AddObject(testObj0), Throws.Exception);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldAddSeveralItemsToRegistry()
        {
            testObj0 = new GameObject("testObj0");
            IUniqueIdentifier uniqueIdentifier0 = testObj0.AddComponent<UniqueId>();
            string id0 = uniqueIdentifier0.GenerateId();
            objectRegistry.AddObject(testObj0);
            var retrievedObj0 = objectRegistry.GetObjectWithGuid(id0);
            Assert.AreEqual(testObj0, retrievedObj0);
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            yield return null;
            testObj1 = new GameObject("testObj1");
            IUniqueIdentifier uniqueIdentifier1 = testObj1.AddComponent<UniqueId>();
            string id1 = uniqueIdentifier1.GenerateId();
            objectRegistry.AddObject(testObj1);
            var retrievedObj1 = objectRegistry.GetObjectWithGuid(id1);
            Assert.AreEqual(testObj1, retrievedObj1);
            Assert.AreEqual(2, objectRegistry.GetObjectsCount());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldRemoveItem()
        {
            testObj0 = new GameObject("testObj0");
            IUniqueIdentifier uniqueIdentifier0 = testObj0.AddComponent<UniqueId>();
            string id = uniqueIdentifier0.GenerateId();
            objectRegistry.AddObject(testObj0);
            var retrievedObj = objectRegistry.GetObjectWithGuid(id);
            Assert.AreEqual(testObj0, retrievedObj);
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            yield return null;

            objectRegistry.RemoveObject(testObj0);
            Assert.AreEqual(0, objectRegistry.GetObjectsCount());
            Assert.IsNull(objectRegistry.GetObjectWithGuid(id));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShouldClearRegistry()
        {
            testObj0 = new GameObject("testObj0");
            IUniqueIdentifier uniqueIdentifier0 = testObj0.AddComponent<UniqueId>();
            uniqueIdentifier0.GenerateId();
            objectRegistry.AddObject(testObj0);
            testObj1 = new GameObject("testObj1");
            IUniqueIdentifier uniqueIdentifier1 = testObj1.AddComponent<UniqueId>();
            uniqueIdentifier1.GenerateId();
            objectRegistry.AddObject(testObj1);
            Assert.AreEqual(2, objectRegistry.GetObjectsCount());
            yield return null;
            objectRegistry.ClearRegistry();
            Assert.AreEqual(0, objectRegistry.GetObjectsCount());
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShouldNotRaiseAnyExeptionIfAttemptToRemoveItemNotInRegistry()
        {
            testObj0 = new GameObject("testObj0");
            testObj0.AddComponent<UniqueId>().GenerateId();
            testObj1 = new GameObject("testObj1");
            objectRegistry.AddObject(testObj0);
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            yield return null;
            objectRegistry.RemoveObject(testObj1);
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            yield return null;
        }

    }
}
