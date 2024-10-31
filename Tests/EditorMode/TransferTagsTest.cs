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
            TagTransferUtils.TransferTags(origin, target);
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
            TagTransferUtils.TransferTags(origin, target);
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
            TagTransferUtils.TransferTags(origin, target);
            Assert.AreEqual(0, tagsController.GetTagsFromObject(target).Count);
            Assert.AreEqual(0, tagsController.GetTagsFromObject(child).Count);
        }

    }

}