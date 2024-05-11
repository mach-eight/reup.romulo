using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;

public class tagsControllerTest : MonoBehaviour
{
    GameObject taggedObject0;
    GameObject taggedObject1;
    GameObject nonTaggedObject0;
    TagsController tagsController;
    ObjectTags objectTags0;
    ObjectTags objectTags1;
    Tag tag0 = new Tag() { id = "tag0", name = "tag0" };
    Tag tag1 = new Tag() { id = "tag1", name = "tag1" };
    Tag tag2 = new Tag() { id = "tag2", name = "tag2" };

    [SetUp]
    public void SetUp()
    {
        tagsController = new TagsController();
        taggedObject0 = new GameObject("taggedObj0");
        objectTags0 = taggedObject0.AddComponent<ObjectTags>();
        objectTags0.AddTags(new Tag[2] {tag0, tag1});
        taggedObject1 = new GameObject("taggedObject1");
        objectTags1 = taggedObject1.AddComponent<ObjectTags>();
        objectTags1.AddTag(tag0);
        nonTaggedObject0 = new GameObject("nonTaggedObj0");
    }
    [TearDown]
    public void TearDown()
    {
        Destroy(taggedObject0);
    }
    [UnityTest]
    public IEnumerator ShouldReturnFalseOnCheckForTags()
    {
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject0, tag2.id));
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject1, tag2.id));
        Assert.IsFalse(tagsController.DoesObjectHaveTag(nonTaggedObject0, tag2.id));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldReturnTrueOnCheckForTags()
    {
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject0, tag0.id));
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject1, tag0.id));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAddTags()
    {
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject0, tag2.id));
        tagsController.AddTagToObject(taggedObject0, tag2);
        yield return null;
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject0, tag2.id));
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldRemoveTags()
    {
        Assert.IsTrue(tagsController.DoesObjectHaveTag(taggedObject0, tag0.id));
        tagsController.RemoveTagFromObject(taggedObject0, tag0);
        yield return null;
        Assert.IsFalse(tagsController.DoesObjectHaveTag(taggedObject0, tag0.id));
        yield return null;
    }

}
