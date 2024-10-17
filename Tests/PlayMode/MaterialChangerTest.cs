using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections.Generic;
using ReupVirtualTwin.helpers;
using System;

public class MaterialChangerTest : MonoBehaviour
{
    IMaterialChanger changer;

    GameObject obj0;
    GameObject obj1;
    GameObject obj2;

    [SetUp]
    public void SetUp()
    {
        obj0 = new GameObject();
        obj0.AddComponent<MeshRenderer>();
        obj1 = new GameObject();
        obj1.AddComponent<MeshRenderer>();
        obj2 = new GameObject();
        changer = new MaterialChanger();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(obj0);
        GameObject.DestroyImmediate(obj1);
        GameObject.DestroyImmediate(obj2);
    }

    [UnityTest]
    public IEnumerator ChangeMaterialOfOneObjectShouldSuccess()
    {
        var materialToSelect = new Material(Shader.Find("Standard"));

        Assert.AreNotEqual(materialToSelect, obj0.GetComponent<Renderer>().sharedMaterial);

        changer.SetNewMaterialToObjects(new List<GameObject>() { obj0 }, new int[1] { 0 }, materialToSelect);
        Assert.AreEqual(materialToSelect, obj0.GetComponent<Renderer>().sharedMaterial);

        yield return null;
    }

    [UnityTest]
    public IEnumerator ChangeMaterialOfSeveralObjectsShouldSuccess()
    {
        var materialToSelect = new Material(Shader.Find("Standard"));

        var objectsList = new List<GameObject>() { obj0, obj1 };
        var indexesList = new int[2] { 0, 0 };

        for (int i = 0; i < objectsList.Count; i++)
        {
            Assert.AreNotEqual(materialToSelect, objectsList[i].GetComponent<Renderer>().sharedMaterial);
        }

        changer.SetNewMaterialToObjects(objectsList, indexesList, materialToSelect);
        for (int i = 0; i < objectsList.Count; i++)
        {
            Assert.AreEqual(materialToSelect, objectsList[i].GetComponent<Renderer>().sharedMaterial);
        }

        yield return null;
    }

    [UnityTest]
    public IEnumerator ChangeMaterialOfObjectWithoutRendererShouldFail()
    {

        var materialToSelect = new Material(Shader.Find("Standard"));


        Assert.That(
            () => changer.SetNewMaterialToObjects(new List<GameObject>() { obj2 }, new int[1] { 0 }, materialToSelect),
            Throws.TypeOf<Exception>()
         );

        yield return null;
    }
}
