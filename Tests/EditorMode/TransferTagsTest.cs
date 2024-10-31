using UnityEngine;
using NUnit.Framework;
using UnityEditor;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.editor;
using ReupVirtualTwin.models;
using System.Collections.Generic;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwinTests.editor
{
    public class TransferTagsTest
    {

        ITagSystemController tagSystemController = new TagSystemController();
        ITagsController tagsController = new TagsController();
        GameObject origin;
        GameObject target;

        public void TearDown()
        {
            GameObject.DestroyImmediate(origin);
            GameObject.DestroyImmediate(target);
        }

        void AssertTagsAreTheSameInHierarchy(GameObject origin, GameObject target)
        {
            List<Tag> originTags = tagsController.GetTagsFromObject(origin);
            List<Tag> targetTags = tagsController.GetTagsFromObject(target);
            Assert.AreEqual(originTags.Count, targetTags.Count);
            foreach (Tag tag in originTags)
            {
                Assert.IsTrue(tagsController.DoesObjectHaveTag(target, tag.id));
            }
            List<GameObject> originChildren = origin.GetChildren();
            List<GameObject> targetChildren = target.GetChildren();
            Dictionary<string, GameObject> targetChildrenByNames = GameObjectUtils.MapGameObjectsByName(targetChildren);
            foreach (GameObject originChild in originChildren)
            {
                GameObject targetChild = targetChildrenByNames[originChild.name];
                if (targetChild != null)
                {
                    AssertTagsAreTheSameInHierarchy(originChild, targetChild);
                }
            }
        }

        void AssertAllObjectsHaveTagSystem(GameObject obj)
        {
            Assert.IsNotNull(obj.GetComponent<ObjectTags>());
            foreach (Transform child in obj.transform)
            {
                AssertAllObjectsHaveTagSystem(child.gameObject);
            }
        }

        [Test]
        public void ShouldAssignTagSystem()
        {
            origin = new GameObject();
            target = new GameObject();
            TransferTagsUtils.TransferTags(origin, target);
            ObjectTags targetObjectTags = target.GetComponent<ObjectTags>();
            Assert.IsNotNull(targetObjectTags);
        }

        [Test]
        public void ShouldAssignTagSystemToAllObjectsInTargetHierarchy()
        {
            origin = new GameObject();
            target = new GameObject();
            GameObject child = new GameObject();
            child.transform.parent = target.transform;
            GameObject grandChild = new GameObject();
            grandChild.transform.parent = child.transform;
            TransferTagsUtils.TransferTags(origin, target);
            AssertAllObjectsHaveTagSystem(target);
        }

        [Test]
        public void ShouldOverrideAnyTagInTarget()
        {
            origin = new GameObject();
            target = new GameObject();
            GameObject child = new GameObject();
            child.transform.parent = target.transform;
            tagSystemController.AssignTagSystemToTree(target);
            List<Tag> tags = TagFactory.CreateBulk(3);
            tagsController.AddTagToObject(target, tags[0]);
            tagsController.AddTagToObject(target, tags[1]);
            Assert.AreEqual(2, tagsController.GetTagsFromObject(target).Count);
            tagsController.AddTagToObject(child, tags[2]);
            Assert.AreEqual(1, tagsController.GetTagsFromObject(child).Count);
            TransferTagsUtils.TransferTags(origin, target);
            Assert.AreEqual(0, tagsController.GetTagsFromObject(target).Count);
            Assert.AreEqual(0, tagsController.GetTagsFromObject(child).Count);
        }

        [Test]
        public void ShouldTransferTagsInParentObject()
        {
            origin = new GameObject();
            target = new GameObject();
            tagSystemController.AssignTagSystemToTree(origin);
            List<Tag> tags = TagFactory.CreateBulk(2);
            tagsController.AddTagsToObject(origin, tags);
            Assert.AreEqual(2, tagsController.GetTagsFromObject(origin).Count);
            TransferTagsUtils.TransferTags(origin, target);
            Assert.IsTrue(tagsController.DoesObjectHaveTag(target, tags[0].id));
            Assert.IsTrue(tagsController.DoesObjectHaveTag(target, tags[1].id));
        }


        [Test]
        public void ShouldTransferTagsInChildAndGrandChildObjects()
        {
            origin = new GameObject();
            target = new GameObject();
            tagSystemController.AssignTagSystemToTree(origin);
            List<Tag> tags = TagFactory.CreateBulk(2);
            tagsController.AddTagsToObject(origin, tags);
            Assert.AreEqual(2, tagsController.GetTagsFromObject(origin).Count);
            TransferTagsUtils.TransferTags(origin, target);
            Assert.IsTrue(tagsController.DoesObjectHaveTag(target, tags[0].id));
            Assert.IsTrue(tagsController.DoesObjectHaveTag(target, tags[1].id));
        }

        [Test]
        public void ShouldTransferTagsInAllObjects()
        {
            string treeString = "(())()";
            origin = GameObjectBuilder.CreateGameObjectHierarchy(treeString);
            target = GameObjectBuilder.CreateGameObjectHierarchy(treeString);
            tagSystemController.AssignTagSystemToTree(origin);
            List<Tag> tags = TagFactory.CreateBulk(4);
            tagsController.AddTagToObject(origin, tags[0]);
            List<GameObject> originChildren = origin.GetChildren();
            tagsController.AddTagToObject(originChildren[0], tags[1]);
            tagsController.AddTagToObject(originChildren[1], tags[2]);
            GameObject grandChild = originChildren[0].GetChildren()[0];
            tagsController.AddTagToObject(grandChild, tags[3]);
            TransferTagsUtils.TransferTags(origin, target);
            AssertTagsAreTheSameInHierarchy(origin, target);
        }

        [Test]
        public void ShouldTransferTagsOnlyToMatchingObjectsByName()
        {
            string treeString = "(())()";
            origin = GameObjectBuilder.CreateGameObjectHierarchy(treeString);
            target = GameObjectBuilder.CreateGameObjectHierarchy(treeString);
            GameObject nonMatchingChild = target.transform.GetChild(1).gameObject;
            nonMatchingChild.name = "nonMatchingChild";
            tagSystemController.AssignTagSystemToTree(origin);
            List<Tag> tags = TagFactory.CreateBulk(4);
            tagsController.AddTagToObject(origin, tags[0]);
            List<GameObject> originChildren = origin.GetChildren();
            tagsController.AddTagToObject(originChildren[0], tags[1]);
            tagsController.AddTagToObject(originChildren[1], tags[2]);
            GameObject grandChild = originChildren[0].GetChildren()[0];
            tagsController.AddTagToObject(grandChild, tags[3]);
            TransferTagsUtils.TransferTags(origin, target);
            Assert.AreEqual(0, tagsController.GetTagsFromObject(nonMatchingChild).Count);
        }

        [Test]
        public void ShouldTransferTagsEvenWithObjectsInDifferentOrder()
        {
            string treeString = "()()";
            origin = GameObjectBuilder.CreateGameObjectHierarchy(treeString);
            target = GameObjectBuilder.CreateGameObjectHierarchy(treeString);
            tagSystemController.AssignTagSystemToTree(origin);
            List<Tag> tags = TagFactory.CreateBulk(2);
            List<GameObject> originChildren = origin.GetChildren();
            originChildren[0].name = "2";
            originChildren[1].name = "1";
            tagsController.AddTagToObject(originChildren[0], tags[0]);
            tagsController.AddTagToObject(originChildren[1], tags[1]);
            TransferTagsUtils.TransferTags(origin, target);
            List<GameObject> targetChildren = target.GetChildren();
            Assert.IsTrue(tagsController.DoesObjectHaveTag(targetChildren[0], tags[1].id));
            Assert.IsTrue(tagsController.DoesObjectHaveTag(targetChildren[1], tags[0].id));
        }
    }

}