using UnityEngine;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using System.Collections.Generic;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.controllers
{
    public class TagFiltersTest : MonoBehaviour
    {
        GameObject taggedObject0;
        GameObject taggedObject1;
        List<Tag> tags;

        [SetUp]
        public void SetUp()
        {
            tags = TagFactory.CreateBulk(2);
            taggedObject0 = new GameObject("taggedObj0");
            taggedObject0.AddComponent<ObjectTags>().AddTag(tags[0]);
            taggedObject1 = new GameObject("taggedObject1");
            taggedObject1.AddComponent<ObjectTags>().AddTags(new Tag[2] { tags[0], tags[1] });
        }
        [TearDown]
        public void TearDown()
        {
            Destroy(taggedObject0);
            Destroy(taggedObject1);
        }
        [Test]
        public void FilterDisplayText_ShouldBeTagName()
        {
            TagFilter filter0 = new TagFilter(tags[0]);
            Assert.AreEqual(filter0.displayText, $"{tags[0].id} {tags[0].name}");
            TagFilter filter1 = new TagFilter(tags[1]);
            Assert.AreEqual(filter1.displayText, $"{tags[1].id} {tags[1].name}");
        }
        [Test]
        public void FilterTagShouldCallOnRemoveCallback()
        {
            TagFilter filter0 = new TagFilter(tags[0]);
            bool callbackCalled = false;
            Assert.IsFalse(callbackCalled);
            filter0.onRemoveFilter = () => { callbackCalled = true; };
            filter0.RemoveFilter();
            Assert.IsTrue(callbackCalled);
        }

    }
}
