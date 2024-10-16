using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;

using ReupVirtualTwin.models;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.helpers;
using Newtonsoft.Json.Linq;
using ReupVirtualTwinTests.utils;

public class ChangeColorObjectsTest : MonoBehaviour
{
    ReupSceneInstantiator.SceneObjects sceneObjects;
    ChangeColorManager changeColorManager;

    GameObject meshedParent;
    GameObject unmeshedParent;
    GameObject unmeshedChild;
    GameObject meshedChild;

    [SetUp]
    public void SetUp()
    {
        sceneObjects = ReupSceneInstantiator.InstantiateScene();
        changeColorManager = sceneObjects.changeColorManager;
        CreateObjects();
    }
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
        Destroy(meshedParent);
        Destroy(unmeshedParent);
        Destroy(meshedChild);
        Destroy(unmeshedChild);
        yield return null;
    }
    private void CreateObjects()
    {
        meshedParent = new GameObject();
        unmeshedParent = new GameObject();
        unmeshedChild = new GameObject();
        meshedChild = new GameObject();

        meshedParent.AddComponent<RegisteredIdentifier>().GenerateId();
        meshedParent.AddComponent<ObjectInfo>();
        meshedParent.AddComponent<MeshRenderer>();

        unmeshedParent.AddComponent<RegisteredIdentifier>().GenerateId();
        unmeshedParent.AddComponent<ObjectInfo>();

        unmeshedChild.AddComponent<RegisteredIdentifier>().GenerateId();
        unmeshedChild.transform.parent = meshedParent.transform;
        unmeshedChild.AddComponent<ObjectInfo>();

        meshedChild.AddComponent<RegisteredIdentifier>().GenerateId();
        meshedChild.AddComponent<MeshRenderer>();
        meshedChild.AddComponent<ObjectInfo>();
        meshedChild.transform.parent = unmeshedParent.transform;
    }

    public List<string> GetIDsArray(List<GameObject> gameObjects)
    {
        List<string> stringIDs = new List<string>();
        foreach (GameObject obj in gameObjects)
        {
            stringIDs.Add(obj.GetComponent<IUniqueIdentifier>().getId());
        }
        return stringIDs;
    }

    [UnityTest]
    public IEnumerator ShouldNotComplain_When_TryingToChangeColorOfUnmeshedObjects()
    {
        List<GameObject> gameObjects = new List<GameObject>() { unmeshedParent, unmeshedChild };
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);
        yield return null;
    }


    [UnityTest]
    public IEnumerator ShouldChangeColorObjects()
    {
        List<GameObject> gameObjects = new() { meshedParent, meshedChild, unmeshedParent, unmeshedChild };
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);

        yield return null;

        Assert.AreEqual(Color.blue, meshedParent.GetComponent<Renderer>().material.color);
        Assert.AreEqual(Color.blue, meshedChild.GetComponent<Renderer>().material.color);

        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldAddColorMetaDataToObjects()
    {
        List<GameObject> gameObjects = new() { meshedParent, meshedChild, unmeshedParent, unmeshedChild };
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);
        yield return null;

        string blueColorRGBA = "#" + ColorUtility.ToHtmlStringRGBA(Color.blue);
        AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<string>(
            gameObjects,
            "appearance.color",
            blueColorRGBA);
        yield return null;

        changeColorManager.ChangeObjectsColor(gameObjects, Color.red);
        yield return null;

        string redColorRGBA = "#" + ColorUtility.ToHtmlStringRGBA(Color.red);
        AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<string>(
            gameObjects,
            "appearance.color",
            redColorRGBA);
        yield return null;
    }
    void AssignFakeMaterialIdMetaDataToObjects(List<GameObject> objects, int materialId)
    {
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].GetComponent<MeshRenderer>() != null)
            {
                ObjectMetaDataUtils.AssignMaterialIdMetaDataToObject(objects[i], materialId);
            }
        }
    }
    [UnityTest]
    public IEnumerator ShouldDeleteMaterialIdMetaData_when_applyingColorMetaData()
    {
        List<GameObject> gameObjects = new() { meshedParent, meshedChild, unmeshedParent, unmeshedChild };
        int fakeMaterialId = 746;
        AssignFakeMaterialIdMetaDataToObjects(gameObjects, fakeMaterialId);
        AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<int>(gameObjects, "appearance.materialId", fakeMaterialId);
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);
        yield return null;
        List<JToken> objectsMaterialId = ObjectMetaDataUtils.GetMetaDataValuesFromObjects(
            gameObjects, "appearance.materialId");
        AssertUtils.AssertAllAreNull(objectsMaterialId);
        AssertUtils.AssertAllObjectsWithMeshRendererHaveMetaDataValue<string>(
            gameObjects,
            "appearance.color",
            "#" + ColorUtility.ToHtmlStringRGBA(Color.blue));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSetChangedMaterialInObjectsInfo()
    {
        Assert.IsFalse(meshedParent.GetComponent<IObjectInfo>().materialWasChanged);
        Assert.IsFalse(unmeshedParent.GetComponent<IObjectInfo>().materialWasChanged);
        Assert.IsFalse(meshedChild.GetComponent<IObjectInfo>().materialWasChanged);
        Assert.IsFalse(unmeshedChild.GetComponent<IObjectInfo>().materialWasChanged);
        List<GameObject> gameObjects = new() { meshedParent, meshedChild, unmeshedParent, unmeshedChild };
        changeColorManager.ChangeObjectsColor(gameObjects, Color.blue);
        yield return null;

        Assert.IsTrue(meshedParent.GetComponent<IObjectInfo>().materialWasChanged);
        Assert.IsTrue(meshedChild.GetComponent<IObjectInfo>().materialWasChanged);

        Assert.IsFalse(unmeshedParent.GetComponent<IObjectInfo>().materialWasChanged);
        Assert.IsFalse(unmeshedChild.GetComponent<IObjectInfo>().materialWasChanged);
    }

}
