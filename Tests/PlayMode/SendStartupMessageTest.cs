using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;

public class SendStartupMessageTest : MonoBehaviour
{
    GameObject startupMessagePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/StartupMessage.prefab");
    GameObject startupMessageContainerGameObject;
    GameObject setupBuildingGameObject;
    GameObject buildingGameObject;
    ISendStartupMessage sendStatusLoadMessage;
    WebMessageSenderSpy webMessageSenderSpy;
    const int BUILDING_CHILDREN_DEPTH = 30;


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        CreateStubSetupBuilding();
        startupMessageContainerGameObject = Instantiate(startupMessagePrefab);
        sendStatusLoadMessage = startupMessageContainerGameObject.GetComponent<ISendStartupMessage>();
        yield return ConfigureAndSetTestSpys();
    }
    [UnityTearDown]
    public IEnumerator TearDown()
    {
        Destroy(startupMessageContainerGameObject);
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

    private IEnumerator ConfigureAndSetTestSpys()
    {
        setupBuildingGameObject.SetActive(false);
        startupMessageContainerGameObject.SetActive(false);
        Destroy((UnityEngine.Object)startupMessageContainerGameObject.GetComponent<IWebMessagesSender>());
        webMessageSenderSpy = startupMessageContainerGameObject.AddComponent<WebMessageSenderSpy>();
        yield return null;
        startupMessageContainerGameObject.SetActive(true);
        setupBuildingGameObject.SetActive(true);
    }

    [UnityTest]
    public IEnumerator ShouldSendsStatusLoadMessageExist()
    {
        Assert.IsNotNull(sendStatusLoadMessage);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendBuildVersion()
    {
        string expectedVersionBuild = sendStatusLoadMessage.buildVersion;
        string buildVersion = ((WebMessage<StartupMessage>)webMessageSenderSpy.sentMessage).payload.buildVersion;
        Assert.AreEqual(expectedVersionBuild, buildVersion);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendBuildingTreeDataStructure()
    {
        ObjectMapper objectMapper = new ObjectMapper(new TagsController(), new IdController());
        ObjectDTO expectedBuildingTreeDataStructure = objectMapper.MapObjectTree(buildingGameObject);
        ObjectDTO buildingTreeDataStructure = ((WebMessage<StartupMessage>)webMessageSenderSpy.sentMessage).payload.building;
        Assert.IsTrue(CompareObjectDTOs(expectedBuildingTreeDataStructure, buildingTreeDataStructure));
        Assert.AreEqual(NumberOfObjectsInTree(expectedBuildingTreeDataStructure), NumberOfObjectsInTree(buildingTreeDataStructure));
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldReturnTotalNumberOfObjects()
    {
        int numberOfObjectsByDefaultInStubBuilding = 4;
        int numberOfObjectsInTotal = numberOfObjectsByDefaultInStubBuilding + BUILDING_CHILDREN_DEPTH;
        ObjectDTO buildingTreeDataStructure = ((WebMessage<StartupMessage>)webMessageSenderSpy.sentMessage).payload.building;
        Assert.AreEqual(numberOfObjectsInTotal, NumberOfObjectsInTree(buildingTreeDataStructure));
        yield return null;
    }

    [UnityTest]
    public IEnumerator MessageToSendMustBeSerialized()
    {
        WebMessage<StartupMessage> message = (WebMessage<StartupMessage>)webMessageSenderSpy.sentMessage;
        string serializedMessage = JsonUtility.ToJson(message);
        WebMessage<StartupMessage> parsedMessage = JsonUtility.FromJson<WebMessage<StartupMessage>>(serializedMessage);
        Assert.AreEqual(message.type, parsedMessage.type);
        Assert.AreEqual(message.payload.buildVersion, parsedMessage.payload.buildVersion);
        Assert.AreEqual(message.payload.building.id, parsedMessage.payload.building.id);
        Assert.AreEqual(message.payload.building.tags.Length, parsedMessage.payload.building.tags.Length);
        yield return null;
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

    private class WebMessageSenderSpy : MonoBehaviour, IWebMessagesSender
    {
        public object sentMessage;
        public void SendWebMessage<T>(WebMessage<T> webMessage)
        {
            sentMessage = webMessage;
        }
    }

}
