using UnityEditor;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using System.Collections;
using System;

using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.models;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviourInterfaces;

public class ReceiveMaterialChangeRequestsTest : MonoBehaviour
{
    GameObject ObjectRegistryPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ObjectRegistry.prefab");
    GameObject objectRegistryGameObject, testObj0, testObj1, requestReceiverGameObject;
    ReceiveMaterialChangeRequests requestReceiver;
    Texture2D testTexture;

    [SetUp]
    public void SetUp()
    {
        testTexture = new Texture2D(100,100);
        requestReceiverGameObject = new GameObject();
        requestReceiverGameObject.AddComponent<MaterialChanger>();
        MockWebRequestTexture webRequestTexture = requestReceiverGameObject.AddComponent<MockWebRequestTexture>();
        webRequestTexture.returnTexture = testTexture;
        requestReceiver = requestReceiverGameObject.AddComponent<ReceiveMaterialChangeRequests>(); 
        objectRegistryGameObject = (GameObject)PrefabUtility.InstantiatePrefab(ObjectRegistryPrefab);
        testObj0 = new GameObject("testObj0");
        testObj1 = new GameObject("testObj1");
        SetObject(testObj0);
        SetObject(testObj1);
    }

    void SetObject(GameObject obj)
    {
        Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        obj.AddComponent<RegisteredIdentifier>();
        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = material;
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(requestReceiverGameObject);
        Destroy(testObj0);
        Destroy(testObj1);
        Destroy(objectRegistryGameObject);
    }

    [UnityTest]
    public IEnumerator ObjectsShouldChangeToNewMaterial()
    {
        Assert.AreNotEqual(testObj0.GetComponent<Renderer>().sharedMaterial.GetTexture("_BaseMap"), testTexture);
        Assert.AreNotEqual(testObj1.GetComponent<Renderer>().sharedMaterial.GetTexture("_BaseMap"), testTexture);

        string id0 = testObj0.GetComponent<RegisteredIdentifier>().getId();
        string id1 = testObj1.GetComponent<RegisteredIdentifier>().getId();

        MaterialChangeRequest request = new MaterialChangeRequest
        {
            objectsIds = new string[] { id0, id1 },
            submeshIndexes = new int[] { 0, 0},
            textureUri = "http://a-texture.url.png"
        };
        string serializedRequest = JsonUtility.ToJson(request);
        yield return requestReceiver.ReceiveSetMaterialRequest(serializedRequest);

        Assert.AreEqual(testTexture, testObj0.GetComponent<Renderer>().sharedMaterial.GetTexture("_BaseMap"));
        Assert.AreEqual(testTexture, testObj1.GetComponent<Renderer>().sharedMaterial.GetTexture("_BaseMap"));

        yield return null;
    }
}

public class MockWebRequestTexture : MonoBehaviour, IWebRequestTexture
{
    Texture2D _returnTexture;
    public Texture2D returnTexture { set =>  _returnTexture = value; }
    public IEnumerator GetTexture(string url, Action<Texture2D> onSuccess, Action<string> onError)
    {
        yield return null;
        onSuccess?.Invoke(_returnTexture);
    }
}
