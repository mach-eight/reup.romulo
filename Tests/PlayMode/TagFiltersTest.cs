using UnityEngine;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwinTests.controllers
{
    public class TagFiltersTest : MonoBehaviour
    {
        GameObject taggedObject0;
        GameObject taggedObject1;
        Tag tag0;
        Tag tag1;

        [SetUp]
        public void SetUp()
        {
            tag0 = new Tag() { id = "tag0", name = "tag0" };
            tag1 = new Tag() { id = "tag1", name = "tag1" };
            taggedObject0 = new GameObject("taggedObj0");
            taggedObject0.AddComponent<ObjectTags>().AddTag(tag0);
            taggedObject1 = new GameObject("taggedObject1");
            taggedObject1.AddComponent<ObjectTags>().AddTags(new Tag[2] { tag0, tag1 });
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
            TagFilter filter0 = new TagFilter(tag0);
            Assert.AreEqual(filter0.displayText, $"{tag0.id} {tag0.name}");
            TagFilter filter1 = new TagFilter(tag1);
            Assert.AreEqual(filter1.displayText, $"{tag1.id} {tag1.name}");
        }
        [Test]
        public void FilterTagShouldCallOnRemoveCallback()
        {
            TagFilter filter0 = new TagFilter(tag0);
            bool callbackCalled = false;
            Assert.IsFalse(callbackCalled);
            filter0.onRemoveFilter = () => { callbackCalled = true; };
            filter0.RemoveFilter();
            Assert.IsTrue(callbackCalled);
        }

    }
}
