using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.managers;

public class FinderControllerTest : MonoBehaviour
{
    GameObject tagsApiManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/TagsApiManager.prefab");
    GameObject tagsApiManagerGameObject;
    TagsApiManager tagsApiManager;

    [SetUp]
    public void SetUp()
    {
        tagsApiManagerGameObject = (GameObject)PrefabUtility.InstantiatePrefab(tagsApiManagerPrefab);
        tagsApiManager = tagsApiManagerGameObject.GetComponent<TagsApiManager>();
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(tagsApiManagerGameObject);
    }


    [Test]
    public void ShouldInjectWebRequesterToTagsApiManagerIfItDoesNotHaveItInjected()
    {
        Assert.IsNull(tagsApiManager.webRequester);
        var foundTagsApiManager = FinderController.FindTagsApiManager();
        Assert.AreEqual(tagsApiManager, foundTagsApiManager);
    }

}
