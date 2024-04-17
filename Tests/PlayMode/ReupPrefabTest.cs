using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

using ReupVirtualTwin.managers;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.behaviourInterfaces;


public class ReupPrefabTest : MonoBehaviour
{
    GameObject reupPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/Quickstart/Reup.prefab");
    GameObject reupGameObject;
    GameObject building;

    IRegistry objectRegistry;
    EditionMediator editionMediator;
    IBuildingGetterSetter setupBuilding;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        CreateComponents();
        CreateBuilding();
        yield return null;
    }

    private void CreateComponents()
    {
        reupGameObject = (GameObject)PrefabUtility.InstantiatePrefab(reupPrefab);
        GameObject baseGlobalScriptGameObject = reupGameObject.transform.Find("BaseGlobalScripts").gameObject;
        objectRegistry = baseGlobalScriptGameObject.transform.Find("ObjectRegistry").GetComponent<IRegistry>();
        editionMediator = baseGlobalScriptGameObject.transform.Find("EditionMediator").GetComponent<EditionMediator>();
        setupBuilding = baseGlobalScriptGameObject.transform.Find("SetupBuilding").GetComponent<IBuildingGetterSetter>();
    }

    private void CreateBuilding()
    {
        building = new GameObject("building");
        building.AddComponent<RegisteredIdentifier>().AssignId("building-id");
        setupBuilding.building = building;
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Destroy(reupGameObject);
        Destroy(building);
        yield return null;
    }

    [UnityTest]
    public IEnumerator EditionMediatorShouldFindTheRegistry()
    {
        Assert.AreEqual(objectRegistry, editionMediator.registry);
        yield return null;
    }
}
