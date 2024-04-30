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
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.enums;

public class ModelInfoManagerTest : MonoBehaviour
{
   GameObject ModelInfoManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Assets/ScriptHolders/ModelInfoManager.prefab");
   GameObject ModelInfoManagerContainerGameObject;
   GameObject setupBuildingGameObject;
   GameObject buildingGameObject;
   WebMessageSenderSpy webMessageSenderSpy;
   const int BUILDING_CHILDREN_DEPTH = 30;


   [UnitySetUp]
   public IEnumerator SetUp()
   {
       CreateStubSetupBuilding();
        ModelInfoManagerContainerGameObject = Instantiate(ModelInfoManagerPrefab);
       yield return ConfigureAndSetTestSpys();
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

   private IEnumerator ConfigureAndSetTestSpys()
   {
       setupBuildingGameObject.SetActive(false);
        ModelInfoManagerContainerGameObject.SetActive(false);
       Destroy((UnityEngine.Object)ModelInfoManagerContainerGameObject.GetComponent<IWebMessagesSender>());
       webMessageSenderSpy = ModelInfoManagerContainerGameObject.AddComponent<WebMessageSenderSpy>();
       yield return null;
       ActivateSetupBuildingFirstThenSendStartupMessage();
   }

   private void ActivateSetupBuildingFirstThenSendStartupMessage()
   {
       setupBuildingGameObject.SetActive(true);
       ModelInfoManagerContainerGameObject.SetActive(true);
   }

   [UnityTest]
   public IEnumerator ShouldObtainTheMessage()
   {
       WebMessage<ModelInfoMessage> message = ((ModelInfoManager)ModelInfoManagerContainerGameObject.GetComponent<IModelInfoManager>()).ObtainModelInfoMessage();
       Assert.IsNotNull(message);
       Assert.AreEqual(WebMessageType.requestModelInfoSuccess, message.type);
       Assert.IsNotNull(message.payload);
       Assert.AreEqual(((ModelInfoManager)ModelInfoManagerContainerGameObject.GetComponent<IModelInfoManager>()).buildVersion, message.payload.buildVersion);
       yield return null;
   }

   [UnityTest]
   public IEnumerator ShouldSendBuildingTreeDataStructure()
   {
       ObjectMapper objectMapper = new ObjectMapper(new TagsController(), new IdController());
       ObjectDTO expectedBuildingTreeDataStructure = objectMapper.MapObjectTree(buildingGameObject);
       WebMessage<ModelInfoMessage> message = ((ModelInfoManager)ModelInfoManagerContainerGameObject.GetComponent<IModelInfoManager>()).ObtainModelInfoMessage();
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
       WebMessage<ModelInfoMessage> message = ((ModelInfoManager)ModelInfoManagerContainerGameObject.GetComponent<IModelInfoManager>()).ObtainModelInfoMessage();
       ObjectDTO buildingTreeDataStructure = message.payload.building;
       Assert.AreEqual(numberOfObjectsInTotal, NumberOfObjectsInTree(buildingTreeDataStructure));
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
