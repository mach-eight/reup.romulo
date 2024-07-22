using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwin.models;
using UnityEditor;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllers;



namespace ReupVirtualTwinTests.Registry
{
    public class IdControllerTest : MonoBehaviour
    {
        GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
        GameObject objectRegistryGameObject;
        IObjectRegistry objectRegistry;
        GameObject parent;
        GameObject child0;
        GameObject child1;
        GameObject grandchild00;
        IdController idController;

        [SetUp]
        public void SetUp()
        {
            objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
            objectRegistry = objectRegistryGameObject.GetComponent<IObjectRegistry>();

            idController = new IdController();
            parent = new GameObject("testObj1");
            child0 = new GameObject("testObj1");
            child1 = new GameObject("testObj2");
            grandchild00 = new GameObject("testObj3");
            child0.transform.parent = parent.transform;
            child1.transform.parent = parent.transform;
            grandchild00.transform.parent = child0.transform;
        }
        
        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            Destroy(parent);
            Destroy(child0);
            Destroy(child1);
            Destroy(grandchild00);
            Destroy(objectRegistryGameObject);
            yield return new WaitForSeconds(0.2f);
        }

        [UnityTest]
        public IEnumerator ShouldAssignObjectInfoToObjects()
        {
            ObjectInfo objectInfo0 = parent.GetComponent<ObjectInfo>();
            Assert.IsNull(objectInfo0);
            yield return null;
            idController.AssignIdToObject(parent);
            objectInfo0 = parent.GetComponent<ObjectInfo>();
            yield return null;
            Assert.IsNotNull(objectInfo0);
            string parentId = objectInfo0.getId();
            Assert.AreEqual(parent, objectRegistry.GetObjectWithGuid(parentId));
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldGenerateNewIdToObject()
        {
            IUniqueIdentifier identifierParent = parent.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            Assert.IsNull(identifierParent);
            Assert.IsNull(identifierChild0);
            yield return null;
            idController.AssignIdToObject(parent);
            identifierParent = parent.GetComponent<IUniqueIdentifier>();
            identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            Assert.IsNotNull(identifierParent);
            Assert.IsNotNull(identifierParent.getId());
            Assert.IsNull(identifierChild0);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldAssignNewIdToObject()
        {
            IUniqueIdentifier identifierParent = parent.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            Assert.IsNull(identifierParent);
            Assert.IsNull(identifierChild0);
            yield return null;
            string parentId = "parent-id";
            idController.AssignIdToObject(parent, parentId);
            identifierParent = parent.GetComponent<IUniqueIdentifier>();
            identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            Assert.IsNotNull(identifierParent);
            Assert.AreEqual(parentId, identifierParent.getId());
            Assert.IsNull(identifierChild0);
        }

        [UnityTest]
        public IEnumerator ShouldAssignIdsToObjectTree()
        {
            IUniqueIdentifier identifierParent = parent.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild1 = child1.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierGrandChild00 = grandchild00.GetComponent<IUniqueIdentifier>();
            Assert.IsNull(identifierParent);
            Assert.IsNull(identifierChild0);
            Assert.IsNull(identifierChild1);
            Assert.IsNull(identifierGrandChild00);
            yield return null;
            idController.AssignIdsToTree(parent);
            identifierParent = parent.GetComponent<IUniqueIdentifier>();
            identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            identifierChild1 = child1.GetComponent<IUniqueIdentifier>();
            identifierGrandChild00 = grandchild00.GetComponent<IUniqueIdentifier>();
            Assert.IsNotNull(identifierParent);
            Assert.IsNotNull(identifierChild0);
            Assert.IsNotNull(identifierChild1);
            Assert.IsNotNull(identifierGrandChild00);
            Assert.IsNotNull(identifierParent.getId());
            Assert.IsNotNull(identifierChild0.getId());
            Assert.IsNotNull(identifierChild1.getId());
            Assert.IsNotNull(identifierGrandChild00.getId());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldAssignIdsToObjectTreeWithParentId()
        {
            IUniqueIdentifier identifierParent = parent.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild1 = child1.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierGrandChild00 = grandchild00.GetComponent<IUniqueIdentifier>();
            Assert.IsNull(identifierParent);
            Assert.IsNull(identifierChild0);
            Assert.IsNull(identifierChild1);
            Assert.IsNull(identifierGrandChild00);
            yield return null;
            string parentId = "parent-id";
            idController.AssignIdsToTree(parent, parentId);
            identifierParent = parent.GetComponent<IUniqueIdentifier>();
            identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            identifierChild1 = child1.GetComponent<IUniqueIdentifier>();
            identifierGrandChild00 = grandchild00.GetComponent<IUniqueIdentifier>();
            Assert.IsNotNull(identifierParent);
            Assert.IsNotNull(identifierChild0);
            Assert.IsNotNull(identifierChild1);
            Assert.IsNotNull(identifierGrandChild00);
            Assert.AreEqual(parentId, identifierParent.getId());
            Assert.AreNotEqual(parentId, identifierChild0.getId());
            Assert.AreNotEqual(parentId, identifierChild1.getId());
            Assert.AreNotEqual(parentId, identifierGrandChild00.getId());
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldNotFindRepeatedIds()
        {
            idController.AssignIdsToTree(parent);
            bool hasRepeteaded = idController.HasRepeatedIds(parent);
            Assert.IsFalse(hasRepeteaded);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldFindRepeatedIds()
        {
            idController.AssignIdsToTree(parent);
            IUniqueIdentifier identifierParent = parent.GetComponent<IUniqueIdentifier>();
            IUniqueIdentifier identifierChild0 = child0.GetComponent<IUniqueIdentifier>();
            string repeatedId = identifierParent.getId();
            identifierChild0.AssignId(repeatedId);
            bool hasRepeteaded = idController.HasRepeatedIds(parent);
            Assert.IsTrue(hasRepeteaded);
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShouldThrow_if_objectDoesNotHaveId()
        {
            Assert.That(() => idController.GetIdFromObject(parent), Throws.Exception);
            yield return null;
        }
    }
}
