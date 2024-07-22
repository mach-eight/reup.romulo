using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.Registry
{
    public class ObjectInfoTest : MonoBehaviour
    {
        GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
        GameObject objectRegistryGameObject;
        IObjectRegistry objectRegistry;
        GameObject testObj;


        [SetUp]
        public void SetUp()
        {
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
            objectRegistry = objectRegistryGameObject.GetComponent<IObjectRegistry>();
            testObj = new GameObject("testObj");
            testObj.AddComponent<ObjectInfo>();
        }

        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            Destroy(testObj);
            objectRegistry.ClearRegistry();
            Destroy(objectRegistryGameObject);
            yield return new WaitForSeconds(0.2f);
        }

        [UnityTest]
        public IEnumerator TestObjHasAnId()
        {
            var id = testObj.GetComponent<ObjectInfo>().getId();
            Assert.IsNotNull(id);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ObjectRegistryContainsTestObj()
        {
            var id = testObj.GetComponent<ObjectInfo>().getId();
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
            string currentId = testObj.GetComponent<ObjectInfo>().getId();
            Assert.AreEqual(testObj, objectRegistry.GetObjectWithGuid(currentId));
            Assert.AreEqual(1, objectRegistry.GetObjectsCount());
            string newId = "new-id";
            testObj.GetComponent<ObjectInfo>().AssignId(newId);
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
            ObjectInfo objectInfo = gameObject.AddComponent<ObjectInfo>();
            objectInfo.GenerateId();
            Assert.IsNotNull(objectInfo.getId());
            Assert.AreEqual(gameObject, objectRegistry.GetObjectWithGuid(objectInfo.getId()));
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShoulBeAbleToAssignIdRightAfterCreatingTheUniqueIdComponent()
        {
            GameObject gameObject = new GameObject("new-game-obj");
            ObjectInfo objectInfo = gameObject.AddComponent<ObjectInfo>();
            string assignedId = "assigned-id";
            objectInfo.AssignId(assignedId);
            Assert.IsNotNull(objectInfo.getId());
            Assert.AreEqual(gameObject, objectRegistry.GetObjectWithGuid(assignedId));
            yield return null;
        }
    }
}
