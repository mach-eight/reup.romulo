using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using ReupVirtualTwinTests.utils;
using ReupVirtualTwinTests.mocks;

public class ModelInfoManagerTest : MonoBehaviour
{
    ObjectMapper objectMapper = new ObjectMapper(new TagsController(), new IdController());
    GameObject ModelInfoManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ModelInfoManager.prefab");
    GameObject ModelInfoManagerContainerGameObject;
    GameObject setupBuildingGameObject;
    GameObject buildingGameObject;
    ModelInfoManager modelInfoManager;
    const int BUILDING_CHILDREN_DEPTH = 30;


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        CreateStubSetupBuilding();
        ModelInfoManagerContainerGameObject = Instantiate(ModelInfoManagerPrefab);
        modelInfoManager = ModelInfoManagerContainerGameObject.GetComponent<ModelInfoManager>();
        yield return null;
    }
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Destroy(ModelInfoManagerContainerGameObject);
        Destroy(setupBuildingGameObject);
        Destroy(buildingGameObject);
        yield return null;
    }
    private void CreateStubSetupBuilding()
    {
        setupBuildingGameObject = StubOnSetupBuildingCreator.CreateImmediateOnSetupBuilding();
        var fakeSetupBuilding = setupBuildingGameObject.GetComponent<StubOnSetupBuildingCreator.FakeSetupBuilding>();
        buildingGameObject = StubObjectTreeCreator.CreateMockBuilding(BUILDING_CHILDREN_DEPTH);
        fakeSetupBuilding.building = buildingGameObject;
    }
    private int NumberOfObjectsInTree(ObjectDTO tree)
    {
        int count = 1;
        foreach (ObjectDTO child in tree.children)
        {
            count += NumberOfObjectsInTree(child);
        }
        return count;
    }

    private bool CompareObjectDTOs(ObjectDTO expected, ObjectDTO obtained)
    {
        if (expected.id != obtained.id)
        {
            return false;
        }
        if (expected.tags.Length != obtained.tags.Length)
        {
            return false;
        }
        for (int i = 0; i < expected.tags.Length; i++)
        {
            if (expected.tags[i] != obtained.tags[i])
            {
                return false;
            }
        }
        if (expected.children.Length != obtained.children.Length)
        {
            return false;
        }
        for (int i = 0; i < expected.children.Length; i++)
        {
            if (!CompareObjectDTOs(expected.children[i], obtained.children[i]))
            {
                return false;
            }
        }
        return true;
    }

    bool IdStructureAreEqual(JObject expected, JObject obtained)
    {
        JObject expectedIdStructure = ExtractIdStructureOnly(expected);
        JObject obtainedIdStructure = ExtractIdStructureOnly(obtained);
        return JToken.DeepEquals(expectedIdStructure, obtainedIdStructure);
    }

    JObject ExtractIdStructureOnly(JObject obj)
    {
        List<JObject> extractedChildren = new List<JObject>();
        JToken children = obj["children"];
        if (children == null)
        {
            return new JObject(new JProperty("id", obj["id"]));
        }
        foreach (JObject child in children)
        {
            extractedChildren.Add(ExtractIdStructureOnly(child));
        }
        return new JObject(
            new JProperty("id", obj["id"]),
            new JProperty("children", new JArray(extractedChildren.ToArray())));
    }

    [UnityTest]
    public IEnumerator ShouldObtainTheModelInfoMessage()
    {
        WebMessage<ModelInfoMessage> message = modelInfoManager.ObtainModelInfoMessage();
        Assert.IsNotNull(message);
        Assert.AreEqual(WebMessageType.requestModelInfoSuccess, message.type);
        Assert.IsNotNull(message.payload);
        Assert.AreEqual(modelInfoManager.buildVersion, message.payload.buildVersion);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldObtainTheUpdateBuildingMessage()
    {
        WebMessage<UpdateBuildingMessage> message = modelInfoManager.ObtainUpdateBuildingMessage();
        Assert.IsNotNull(message);
        Assert.AreEqual(WebMessageType.updateBuilding, message.type);
        Assert.IsNotNull(message.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldObtainBuildingTreeDataStructure()
    {
        ObjectDTO expectedBuildingTreeDataStructure = objectMapper.MapObjectTree(buildingGameObject);
        WebMessage<ModelInfoMessage> message = modelInfoManager.ObtainModelInfoMessage();
        ObjectDTO buildingTreeDataStructure = message.payload.building;
        Assert.IsTrue(CompareObjectDTOs(expectedBuildingTreeDataStructure, buildingTreeDataStructure));
        Assert.AreEqual(NumberOfObjectsInTree(expectedBuildingTreeDataStructure), NumberOfObjectsInTree(buildingTreeDataStructure));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldReturnTotalNumberOfObjects()
    {
        int numberOfObjectsByDefaultInStubBuilding = 4;
        int numberOfObjectsInTotal = numberOfObjectsByDefaultInStubBuilding + BUILDING_CHILDREN_DEPTH;
        WebMessage<ModelInfoMessage> message = modelInfoManager.ObtainModelInfoMessage();
        ObjectDTO buildingTreeDataStructure = message.payload.building;
        Assert.AreEqual(numberOfObjectsInTotal, NumberOfObjectsInTree(buildingTreeDataStructure));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldGetSceneStateMessage_with_SameIdStructureAsBuilding()
    {
        JObject sceneSTate = modelInfoManager.GetSceneState();
        ObjectDTO buildingTreeDataStructure = objectMapper.MapObjectTree(buildingGameObject);
        Assert.IsTrue(IdStructureAreEqual(JObject.FromObject(buildingTreeDataStructure), sceneSTate));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldGetSceneStateMessage_with_noAppearanceInfoAtTheBeginning()
    {
        JObject sceneSTate = modelInfoManager.GetSceneState();
        Assert.IsNull(sceneSTate["appearance"]);
        Assert.IsNull(sceneSTate["children"][0]["appearance"]);
        Assert.IsNull(sceneSTate["children"][1]["appearance"]);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldGetColorInfoInSceneStateMessage()
    {
        MetaDataComponentMock metaDataComponent = buildingGameObject.AddComponent<MetaDataComponentMock>();
        JObject parentMetaData = new JObject
        {
            { "appearance", new JObject
                {
                    { "color", "test-color-parent" }
                }
            }
        };
        metaDataComponent.objectMetaData = parentMetaData;

        MetaDataComponentMock metaDataChildComponent = buildingGameObject.transform.GetChild(0).gameObject.AddComponent<MetaDataComponentMock>();
        JObject childMetaData = new JObject
        {
            { "appearance", new JObject
                {
                    { "color", "test-color-child" }
                }
            }
        };
        metaDataChildComponent.objectMetaData = childMetaData;

        JObject sceneSTate = modelInfoManager.GetSceneState();
        Assert.AreEqual(parentMetaData["appearance"]["color"], sceneSTate["appearance"]["color"]);
        Assert.AreEqual(childMetaData["appearance"]["color"], sceneSTate["children"][0]["appearance"]["color"]);
        yield return null;
    }

}
