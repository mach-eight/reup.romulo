using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;


public class ObjectTagsTest : MonoBehaviour
{
    GameObject containerGameObject;
    ObjectTags objectTags;
    Tag tag;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject("container");
        objectTags = containerGameObject.AddComponent<ObjectTags>();
        tag = new Tag() { id = "tag-id" };
    }
    [TearDown]
    public void TearDown()
    {
        Destroy(containerGameObject);
    }
    [UnityTest]
    public IEnumerator ShouldInitializeWithEmptyTagsList()
    {
        Assert.IsEmpty(objectTags.GetTags());
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAddOneTag()
    {
        objectTags.AddTag(tag);
        Assert.AreEqual(1, objectTags.GetTags().Count);
        Assert.IsTrue(objectTags.GetTags().Contains(tag));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldRemoveOneTag()
    {
        objectTags.AddTag(tag);
        Assert.AreEqual(1, objectTags.GetTags().Count);
        Assert.IsTrue(objectTags.GetTags().Contains(tag));
        yield return null;
        objectTags.RemoveTag(tag);
        Assert.AreEqual(0, objectTags.GetTags().Count);
        Assert.IsFalse(objectTags.GetTags().Contains(tag));
        yield return null;
    }
}
