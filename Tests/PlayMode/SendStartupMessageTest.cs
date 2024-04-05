using System;
using System.Collections;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviours;

public class SendStartupMessageTest : MonoBehaviour
{
    GameObject statusLoadMessagePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/StatusLoadMessage.prefab");
    GameObject statusLoadMessageObject;
    GameObject setupBuildingGameObject;
    ISendStartupMessage sendStatusLoadMessage;
    WebMessageSenderSpy webMessageSenderSpy;


    [UnitySetUp]
    public IEnumerator SetUp()
    {
        StubOnSetupBuildingCreator.CreateOnSetupBuilding();
        statusLoadMessageObject = Instantiate(statusLoadMessagePrefab);
        yield return SetTestSpys();
        sendStatusLoadMessage = statusLoadMessageObject.GetComponent<ISendStartupMessage>();
    }

    private IEnumerator SetTestSpys()
    {
        statusLoadMessageObject.SetActive(false);
        Destroy((UnityEngine.Object)statusLoadMessageObject.GetComponent<IWebMessagesSender>());
        webMessageSenderSpy = statusLoadMessageObject.AddComponent<WebMessageSenderSpy>();
        yield return null;
        statusLoadMessageObject.SetActive(true);
    }

    [UnityTest]
    public IEnumerator ShouldSendsStatusLoadMessageExist()
    {
        Assert.IsNotNull(sendStatusLoadMessage);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendsModelVersionNumber()
    {
        string expectedVersionBuild = sendStatusLoadMessage.version_build;
        string versionBuild = ((WebMessage<string>)webMessageSenderSpy.sentMessage).payload;
        Assert.AreEqual(expectedVersionBuild, versionBuild);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendBuildingTreeDataStructure()
    {
        ObjectMapper objectMapper = new ObjectMapper(new TagsController(), new IdController());
        ObjectDTO expectedBuildingTreeDataStructure = objectMapper.MapObjectTree(StubObjectTreeCreator.CreateMockBuilding());
        ObjectDTO buildingTreeDataStructure = ((WebMessage<StartupMessage>)webMessageSenderSpy.sentMessage).payload.building;
        Assert.AreEqual(expectedBuildingTreeDataStructure, buildingTreeDataStructure);
        yield return null;
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
