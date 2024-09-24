using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using ReupVirtualTwin.models;
using UnityEditor;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwinTests.utils;



namespace ReupVirtualTwinTests.Registry
{
    public class IdControllerTest : MonoBehaviour
    {
        ReupSceneInstantiator.SceneObjects sceneObjects;
        IObjectRegistry objectRegistry;
        GameObject parent;
        GameObject child0;
        GameObject child1;
        GameObject grandchild00;
        IdController idController;

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateScene();
            objectRegistry = sceneObjects.objectRegistry;

            idController = new IdController();
            parent = new GameObject("testObj1");
            child0 = new GameObject("testObj1");
            child1 = new GameObject("testObj2");
            grandchild00 = new GameObject("testObj3");
            child0.transform.parent = parent.transform;
            child1.transform.parent = parent.transform;
            grandchild00.transform.parent = child0.transform;
            yield return null;
        }
        
        [UnityTearDown]
        public IEnumerator TearDownCoroutine()
        {
            Destroy(parent);
            Destroy(child0);
            Destroy(child1);
            Destroy(grandchild00);
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [UnityTest]
        public IEnumerator ShouldAssignARegisteredIdentifier()
        {
            RegisteredIdentifier registeredIdentifier0 = parent.GetComponent<RegisteredIdentifier>();
            Assert.IsNull(registeredIdentifier0);
            yield return null;
            idController.AssignIdToObject(parent);
            registeredIdentifier0 = parent.GetComponent<RegisteredIdentifier>();
            yield return null;
            Assert.IsNotNull(registeredIdentifier0);
            string parentId = registeredIdentifier0.getId();
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
