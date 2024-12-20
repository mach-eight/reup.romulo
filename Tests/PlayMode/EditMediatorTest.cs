using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ReupVirtualTwin.managers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataSchemas;
using Newtonsoft.Json.Schema;
using ReupVirtualTwinTests.mocks;
using ReupVirtualTwinTests.utils;

public class EditMediatorTest : MonoBehaviour
{
    GameObject containerGameObject;
    EditMediator editMediator;
    MockEditModeManager mockEditModeManager;
    MockSelectedObjectsManager mockSelectedObjectsManager;
    MockWebMessageSender mockWebMessageSender;
    MockTransformObjectsManager mockTransformObjectsManager;
    MockInsertObjectsManager mockInsertObjectsManager;
    MockObjectMapper mockObjectMapper;
    ObjectRegistrySpy registrySpy;
    ChangeColorManagerSpy changeColorManagerSpy;
    ChangeMaterialControllerSpy changeMaterialControllerSpy;
    MockModelInfoManager mockModelInfoManager;
    JObject requestSceneLoadMessage;
    OriginalSceneControllerSpy originalSceneControllerSpy;
    BuildingVisibilityControllerSpy buildingVisibilityControllerSpy;
    ViewModeManagerSpy viewModeManagerSpy;

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject();
        editMediator = containerGameObject.AddComponent<EditMediator>();
        mockEditModeManager = new MockEditModeManager();
        mockSelectedObjectsManager = new MockSelectedObjectsManager();
        editMediator.editModeManager = mockEditModeManager;
        editMediator.selectedObjectsManager = mockSelectedObjectsManager;
        mockWebMessageSender = new MockWebMessageSender();
        editMediator.webMessageSender = mockWebMessageSender;
        mockTransformObjectsManager = new MockTransformObjectsManager();
        editMediator.transformObjectsManager = mockTransformObjectsManager;
        mockInsertObjectsManager = new MockInsertObjectsManager(editMediator);
        editMediator.insertObjectsController = mockInsertObjectsManager;
        mockObjectMapper = new MockObjectMapper();
        editMediator.objectMapper = mockObjectMapper;
        registrySpy = new ObjectRegistrySpy();
        editMediator.registry = registrySpy;
        changeColorManagerSpy = new ChangeColorManagerSpy();
        editMediator.changeColorManager = changeColorManagerSpy;
        changeMaterialControllerSpy = new ChangeMaterialControllerSpy();
        editMediator.changeMaterialController = changeMaterialControllerSpy;
        mockModelInfoManager = new MockModelInfoManager();
        editMediator.modelInfoManager = mockModelInfoManager;
        requestSceneLoadMessage = new JObject(
            new JProperty("type", WebMessageType.requestSceneLoad),
            new JProperty("payload", new JObject()
            {
                { "requestTimestamp", 1723834815802 },
                { "objects",  null },
            })
        );
        originalSceneControllerSpy = new OriginalSceneControllerSpy();
        editMediator.originalSceneController = originalSceneControllerSpy;
        buildingVisibilityControllerSpy = new BuildingVisibilityControllerSpy();
        editMediator.buildingVisibilityController = buildingVisibilityControllerSpy;
        viewModeManagerSpy = new ViewModeManagerSpy();
        editMediator.viewModeManager = viewModeManagerSpy;
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(containerGameObject);
        registrySpy.DestroyTestObjects();
        mockInsertObjectsManager.DestroyTestObjects();
        mockSelectedObjectsManager.DestroyTestObjects();
    }

    private class ViewModeManagerSpy : IViewModeManager
    {
        public ViewMode lastRequestedViewMode;
        public bool throwError = false;
        public string errorMessage = "Error, Fail to change view mode from spy";
        public void ActivateDHV()
        {
            if (throwError)
            {
                throw new Exception(errorMessage);
            }
            lastRequestedViewMode = ViewMode.dollhouse;
        }

        public void ActivateFPV()
        {
            if (throwError)
            {
                throw new Exception(errorMessage);
            }
            lastRequestedViewMode = ViewMode.firstPerson;
        }
    }

    private class OriginalSceneControllerSpy : IOriginalSceneController
    {
        public int restoreOriginalSceneCallsCount = 0;
        public void RestoreOriginalScene()
        {
            restoreOriginalSceneCallsCount++;
        }
    }

    private class ChangeColorManagerSpy : IChangeColorManager
    {
        public List<GameObject> lastCalledObjects;
        public Color lastCalledColor;
        public List<Color> calledColors = new List<Color>();
        public List<List<GameObject>> calledObjects = new List<List<GameObject>>();
        public void ChangeObjectsColor(List<GameObject> objectsToDelete, Color color)
        {
            lastCalledObjects = objectsToDelete;
            lastCalledColor = color;
            calledColors.Add(color);
            calledObjects.Add(objectsToDelete);
        }
    }

    private class ChangeMaterialControllerSpy : IChangeMaterialController
    {
        public JObject lastReceivedMessageRequest;
        public List<JObject> receivedMessageRequests = new List<JObject>();
        public bool throwError = false;
        public Task<TaskResult> ChangeObjectMaterial(JObject materialChangeInfo)
        {
            if (throwError)
            {
                return Task.FromResult(TaskResult.Failure("ERROR, Fail to change materials from spy"));
            }

            bool isValid = materialChangeInfo.IsValid(RomuloInternalSchema.materialChangeInfoSchema, out IList<string> errorMessages);
            if (!isValid)
            {
                foreach (string errorMessage in errorMessages)
                {
                    Debug.LogError(errorMessage);
                }
                throw new Exception("Invalid material change info object");
            }
            receivedMessageRequests.Add(materialChangeInfo);
            lastReceivedMessageRequest = materialChangeInfo;

            return Task.FromResult(TaskResult.Success());
        }
    }

    private class MockEditModeManager : IEditModeManager
    {
        private bool _editMode;
        public bool editMode { get => _editMode; set => _editMode = value; }
    }
    private class MockSelectedObjectsManager : ISelectedObjectsManager
    {
        public MockSelectedObjectsManager()
        {
            wrapperDTO = new ObjectWrapperDTO()
            {
                wrapper = null,
                wrappedObjects = null,
            };
        }
        private bool _allowEditSelection = false;
        public bool allowEditSelection { get => _allowEditSelection; set => _allowEditSelection = value; }


        public bool selectionCleared = false;
        private ObjectWrapperDTO _wraperDTO;
        public ObjectWrapperDTO wrapperDTO { get => _wraperDTO; set => _wraperDTO = value; }

        public GameObject AddObjectToSelection(GameObject selectedObject)
        {
            if (wrapperDTO.wrapper == null)
            {
                wrapperDTO = new ObjectWrapperDTO()
                {
                    wrapper = new GameObject("wrapper"),
                    wrappedObjects = new List<GameObject>(),
                };
            }
            wrapperDTO.wrappedObjects.Add(selectedObject);
            return wrapperDTO.wrapper;
        }

        public void ClearSelection()
        {
            selectionCleared = true;
        }

        public GameObject RemoveObjectFromSelection(GameObject selectedObject)
        {
            throw new System.NotImplementedException();
        }

        public GameObject ForceRemoveObjectFromSelection(GameObject selectedObject)
        {
            throw new System.NotImplementedException();
        }

        public void DestroyTestObjects()
        {
            if (wrapperDTO != null && wrapperDTO.wrapper != null)
            {
                GameObject.DestroyImmediate(wrapperDTO.wrapper);
                if (wrapperDTO.wrappedObjects != null)
                {
                    foreach (GameObject obj in wrapperDTO.wrappedObjects)
                    {
                        GameObject.DestroyImmediate(obj);
                    }
                }
            }
        }
    }

    private class MockModelInfoManager : IModelInfoManager, ISceneStateManager
    {
        public ObjectDTO building;
        public bool sceneStateRequested = false;
        public MockModelInfoManager()
        {
            building = new ObjectDTO
            {
                id = "building-id",
                tags = TagFactory.CreateBulk(2).ToArray(),
            };
        }

        public WebMessage<JObject> ObtainModelInfoMessage()
        {
            WebMessage<JObject> message = new()
            {
                type = WebMessageType.requestModelInfoSuccess,
                payload = new JObject()
                {
                    {"buildVersion", "2024-04-05"},
                    {"building", JObject.FromObject(building)},
                },
            };
            return message;
        }

        public WebMessage<JObject> ObtainUpdateBuildingMessage()
        {
            WebMessage<JObject> message = new()
            {
                type = WebMessageType.updateBuilding,
                payload = new JObject()
                {
                    {"building", JObject.FromObject(building)},
                },
            };
            return message;
        }

        public void InsertObjectToBuilding(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        public JObject GetSceneState()
        {
            sceneStateRequested = true;
            return new JObject();
        }
    }

    private class MockWebMessageSender : IWebMessagesSender
    {
        public List<object> sentMessages = new List<object>();

        public void SendWebMessage<T>(WebMessage<T> webMessage)
        {
            sentMessages.Add(webMessage);
        }
    }
    private class MockTransformObjectsManager : ITransformObjectsManager
    {
        public bool _active = false;
        public bool active => _active;

        public ObjectWrapperDTO wrapper { set => throw new System.NotImplementedException(); }

        public void ActivateTransformMode(GameObject wrapper, TransformMode mode)
        {
            _active = true;
        }

        public void ActivateTransformMode(ObjectWrapperDTO wrapper, TransformMode mode)
        {
            throw new System.NotImplementedException();
        }

        public void DeactivateTransformMode()
        {
            _active = false;
        }
    }

    private class MockInsertObjectsManager : IInsertObjectsController
    {
        public GameObject injectedObject = null;
        public bool calledToInsertObject = false;
        public string objectLoadString = null;
        public string requestedObjectId;
        private IMediator editMediator;
        public MockInsertObjectsManager(IMediator mediator)
        {
            calledToInsertObject = false;
            objectLoadString = null;
            editMediator = mediator;
        }

        public void InsertObject(InsertObjectMessagePayload insertObjectMessagePayload)
        {
            injectedObject = new GameObject("injected test object");
            calledToInsertObject = true;
            objectLoadString = insertObjectMessagePayload.objectUrl;
            InsertedObjectPayload insertedObjectPayload = new()
            {
                loadedObject = injectedObject,
                selectObjectAfterInsertion = insertObjectMessagePayload.selectObjectAfterInsertion,
            };
            editMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
            requestedObjectId = insertObjectMessagePayload.objectId;
        }

        public void DestroyTestObjects()
        {
            GameObject.DestroyImmediate(injectedObject);
        }
    }
    private class MockObjectMapper : IObjectMapper
    {
        public ObjectDTO[] objectDTOs = new ObjectDTO[2]
        {
            new ObjectDTO
            {
                id = "id0",
                tags = TagFactory.CreateBulk(2).ToArray(),
            },
            new ObjectDTO
            {
                id = "id1",
                tags = TagFactory.CreateBulk(2).ToArray(),
            },
        };

        public JObject GetObjectSceneState(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        public JObject GetTreeSceneState(GameObject obj)
        {
            throw new System.NotImplementedException();
        }

        public ObjectDTO[] MapObjectsToDTO(List<GameObject> objs)
        {
            return objectDTOs;
        }

        public ObjectDTO MapObjectToDTO(GameObject obj)
        {
            return objectDTOs[0];
        }

        public ObjectDTO MapObjectTree(GameObject obj)
        {
            throw new System.NotImplementedException();
        }
    }

    private class BuildingVisibilityControllerSpy : IBuildingVisibilityController
    {
        public List<string> lastRequestedObjectIds;
        public bool lastRequestedVisibility;
        public bool showAllObjectsCalled = false;
        public bool error = false;
        public TaskResult SetObjectsVisibility(string[] objectsIds, bool show)
        {
            if (error)
            {
                return TaskResult.Failure("Error, Fail to change visibility from spy");
            }
            lastRequestedObjectIds = new List<string>(objectsIds);
            lastRequestedVisibility = show;
            return TaskResult.Success();
        }

        public TaskResult ShowAllObjects()
        {
            if (error)
            {
                return TaskResult.Failure("Error, Fail to change visibility from spy");
            }
            showAllObjectsCalled = true;
            return TaskResult.Success();
        }

        public TaskResult HideAllObjects()
        {
            throw new System.NotImplementedException();
        }
    }
    private static class dummyJsonCreator
    {
        public static string createWebMessage(string type)
        {
            return $"{{\"type\":\"{type}\"}}";
        }
        public static string createWebMessage(string type, object payload)
        {
            if (payload is int || payload is float)
            {
                return $"{{\"type\":\"{type}\",\"payload\":{payload}}}";
            }
            string processedPayload;
            if (payload is string)
            {
                processedPayload = payload.ToString();
            }
            else if (payload is bool)
            {
                processedPayload = payload.ToString().ToLower();
            }
            else
            {
                processedPayload = JsonUtility.ToJson(payload);
            }
            processedPayload = ScapeSpecialChars(processedPayload);
            return $"{{\"type\":\"{type}\",\"payload\":\"{processedPayload}\"}}";

        }
        static string ScapeSpecialChars(string str)
        {
            return str.Replace("\"", "\\\"");
        }
    }

    private void AssertExpectedData(object expected, object obtained)
    {
        var proccessedExpected = JToken.Parse(JsonConvert.SerializeObject(expected));
        var proccessedObtained = JToken.Parse(JsonConvert.SerializeObject(obtained));
        Assert.AreEqual(proccessedExpected, proccessedObtained);
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToTrue()
    {
        editMediator.Notify(ReupEvent.setEditMode, true);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(true, sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToFalse()
    {
        editMediator.Notify(ReupEvent.setEditMode, false);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(false, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSetEditModeWhenReceiveRequest()
    {
        string message = JObject.FromObject(new { type = WebMessageType.setEditMode, payload = true }).ToString();
        editMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, true);
        yield return null;
        message = JObject.FromObject(new { type = WebMessageType.setEditMode, payload = false }).ToString();
        editMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, false);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAllowAndDisallowObjectSelection()
    {
        editMediator.Notify(ReupEvent.setEditMode, true);
        Assert.AreEqual(mockSelectedObjectsManager.allowEditSelection, true);
        yield return null;
        editMediator.Notify(ReupEvent.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.allowEditSelection, false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldAllowSelection()
    {
        string message = JObject.FromObject(new { type = WebMessageType.allowSelection }).ToString();
        editMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockSelectedObjectsManager.allowEditSelection, true);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldDisableSelection()
    {
        string message = JObject.FromObject(new { type = WebMessageType.disableSelection }).ToString();
        editMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockSelectedObjectsManager.allowEditSelection, false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearSelectionWhenEditModeIsSetToFalse()
    {
        editMediator.Notify(ReupEvent.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.selectionCleared, true);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToActivatePositionTransformModeWithNoSelectedObject()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.activatePositionTransform);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual($"Can't activate {TransformMode.PositionMode} Transform mode because no object is selected", sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToActivateRotationTransformModeWithNoSelectedObject()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.activateRotationTransform);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual($"Can't activate {TransformMode.RotationMode} Transform mode because no object is selected", sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToDeactivateTransformModeButNoTransforModeIsActive()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.deactivateTransformMode);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual("Can't deactivate transform mode if no transform mode is currently active", sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldOrderInsertObjectManagerToInsertObject()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-object-id",
            selectObjectAfterInsertion = true,
            deselectPreviousSelection = true,
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsTrue(mockInsertObjectsManager.calledToInsertObject);
        Assert.AreEqual(payload.objectUrl, mockInsertObjectsManager.objectLoadString);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectUpdatedProcess()
    {
        float processStatus = 0.25f;
        editMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        WebMessage<float> sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
        processStatus = 0.6f;
        editMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessages[1];
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectSuccessAndUpdateBuildingMessage()
    {
        GameObject mockInsertedObject = new GameObject("insertedObject");
        InsertedObjectPayload insertedObjectPayload = new InsertedObjectPayload()
        {
            loadedObject = mockInsertedObject,
        };
        editMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        yield return null;
        WebMessage<ObjectDTO> sentMessage = (WebMessage<ObjectDTO>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.loadObjectSuccess, sentMessage.type);
        Assert.AreEqual(mockObjectMapper.objectDTOs[0], sentMessage.payload);
        WebMessage<JObject> sentUpdateBuildingMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[1];
        Assert.AreEqual(WebMessageType.updateBuilding, sentUpdateBuildingMessage.type);
        Assert.IsTrue(JObject.DeepEquals(JObject.FromObject(mockModelInfoManager.building), sentUpdateBuildingMessage.payload["building"]));
        GameObject.DestroyImmediate(mockInsertedObject);
    }

    [UnityTest]
    public IEnumerator ShouldNotSelectInsertedObjectByDefault()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-3d-model-url",
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsNull(mockSelectedObjectsManager.wrapperDTO.wrappedObjects);
        Assert.IsNull(mockSelectedObjectsManager.wrapperDTO.wrappedObjects);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceiveInsertObjectActionWithoutObjectUrl()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectId = "test-3d-model-url",
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editMediator.noInsertObjectUrlErrorMessage, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceiveInsertObjectActionWithoutObjectId()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editMediator.noInsertObjectIdErrorMessage, sentMessage.payload);
        yield return null;
    }

    [Test]
    public void ShouldSendSuccessMessage_When_LoadSceneIsSuccessful()
    {
        string color = "#00FF00";
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", color },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material", null }
                }
            }
        );

        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];

        Assert.AreEqual(WebMessageType.requestSceneLoadSuccess, sentMessage.type);
    }

    [Test]
    public void ShouldSendFailureMessage_When_LoadSceneFails()
    {
        JObject material = new JObject()
        {
            { "id", 1 },
            { "texture", "material-1-url" },
            { "widthMillimeters", 2000 },
            { "heightMillimeters", 1500}
        };
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", null },
                    { "material", material }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material", null }
                }
            }
        );

        changeMaterialControllerSpy.throwError = true;
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        string actualErrorMessage = sentMessage.payload["errorMessage"].ToString();

        Assert.AreEqual(WebMessageType.requestSceneLoadFailure, sentMessage.type);
        Assert.AreEqual("ERROR, Fail to change materials from spy", actualErrorMessage);
    }

    [UnityTest]
    public IEnumerator ShouldSelectJustInsertedObject()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-object-id",
            selectObjectAfterInsertion = true,
            deselectPreviousSelection = true,
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsTrue(mockSelectedObjectsManager.wrapperDTO.wrappedObjects.Contains(mockInsertObjectsManager.injectedObject));
        Assert.AreEqual(1, mockSelectedObjectsManager.wrapperDTO.wrappedObjects.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldRequestAddPredefinedIdToParentObject()
    {
        InsertObjectMessagePayload payload = new InsertObjectMessagePayload
        {
            objectUrl = "test-3d-model-url",
            objectId = "test-object-id",
            selectObjectAfterInsertion = true,
            deselectPreviousSelection = true,
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.loadObject, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.AreEqual(payload.objectId, mockInsertObjectsManager.requestedObjectId);
    }

    [UnityTest]
    public IEnumerator ShouldSendRequestToChangeObjectsColor_When_ReceiveChangeObjectColorMessage()
    {
        string color = "#00FF00";
        Color expectedColor = Color.green;
        ChangeColorObjectMessagePayload payload = new ChangeColorObjectMessagePayload
        {
            color = color,
            objectIds = new string[] { "id-0", "id-1" },
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.changeObjectColor, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.AreEqual(registrySpy.lastRequestedObjectIds, payload.objectIds);
        Assert.AreEqual(expectedColor, changeColorManagerSpy.lastCalledColor);
        Assert.AreEqual(registrySpy.objects, changeColorManagerSpy.lastCalledObjects);
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_When_IncorrectColorCodeIsReceivedInChangeColorRequest()
    {
        string color = "this is not a proper color code";
        ChangeColorObjectMessagePayload payload = new ChangeColorObjectMessagePayload
        {
            color = color,
            objectIds = new string[] { "id-0", "id-1" },
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.changeObjectColor, payload);
        editMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editMediator.InvalidColorErrorMessage(color), sentMessage.payload);
    }

    [UnityTest]
    public IEnumerator ShouldRequestChangeMaterialOfObjects_When_ReceivesChangeMaterialRequest()
    {
        JObject material = new JObject()
        {
            { "id", 123456 },
            { "texture", "material-1-url" },
            { "widthMillimeters", 2000 },
            { "heightMillimeters", 1500 }
        };
        JObject message = new JObject()
        {
            { "type", WebMessageType.changeObjectsMaterial },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(new string[] { "id-0", "id-1" }) },
                    { "material", material },
                }
            }
        };
        var serializedMessage = JsonConvert.SerializeObject(message);
        editMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        AssertExpectedData(message["payload"], changeMaterialControllerSpy.lastReceivedMessageRequest);
    }


    [UnityTest]
    public IEnumerator ShouldSendSuccessMessage_When_NotifiedOfMaterialChangeSuccess()
    {
        JObject material = new JObject()
        {
            { "id", 12345 },
            { "texture", "material-1-url" },
            { "widthMillimeters", 2000 },
            { "heightMillimeters", 1500}
        };
        JObject materialChangeInfo = new JObject(
            new JProperty("material", material),
            new JProperty("objectIds", new JArray(new string[] { "id-0", "id-1" }))
        );
        editMediator.Notify(ReupEvent.objectMaterialChanged, materialChangeInfo);
        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.changeObjectsMaterialSuccess, sentMessage.type);
        yield return null;
    }

    [Test]
    public void ShouldSendFailureMessage_When_NotifiedOfMaterialCannotChange()
    {
        JObject material = new JObject()
        {
            { "id", 1 },
            { "texture", "material-1-url" },
            { "widthMillimeters", 2000 },
            { "heightMillimeters", 1500}
        };

        JObject requesMessage = new JObject(
           new JProperty("type", WebMessageType.changeObjectsMaterial),
           new JProperty("payload", new JObject()
           {
               new JProperty("material", material),
               new JProperty("objectIds", new JArray(new string[] { "id-0", "id-1" }))
           })
       );

        changeMaterialControllerSpy.throwError = true;
        string serializedMessage = JsonConvert.SerializeObject(requesMessage);
        editMediator.ReceiveWebMessage(serializedMessage);

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.changeObjectsMaterialFailure, sentMessage.type);
    }

    [UnityTest]
    public IEnumerator ShouldReturnErrorMessage_when_ReceiveWrongRequestMaterialChangeMessage()
    {
        JObject message = new JObject(
            new JProperty("type", WebMessageType.changeObjectsMaterial),
            new JProperty("payload", new JObject(
                new JProperty("misspelled_material_url", "material-url"),
                new JProperty("misspelled_object_ids", new JArray(new string[] { "id-0", "id-1" }))
            ))
        );
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldReturnErrorMessage_when_receiveInvalidTypeInMessage()
    {
        JObject message = new JObject
        {
            { "type", "invalid-type" }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendDeleteObjectSuccessAndUpdateBuildingMessage()
    {
        editMediator.Notify(ReupEvent.objectsDeleted);
        WebMessage<string> deletedSuccessMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        yield return null;
        Assert.AreEqual(WebMessageType.deleteObjectsSuccess, deletedSuccessMessage.type);
        WebMessage<JObject> sentUpdateBuildingMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[1];
        Assert.AreEqual(WebMessageType.updateBuilding, sentUpdateBuildingMessage.type);
        Assert.IsTrue(JObject.DeepEquals(JObject.FromObject(mockModelInfoManager.building), sentUpdateBuildingMessage.payload["building"]));
    }

    [UnityTest]
    public IEnumerator ShouldRejectSceneStateMessage_when_noSceneNameProvided()
    {
        WebMessage<Dictionary<string, object>> sceneStateRequestMessage = new WebMessage<Dictionary<string, object>>()
        {
            type = WebMessageType.requestSceneState,
            payload = new Dictionary<string, object>()
        };
        var serializedMessage = JsonConvert.SerializeObject(sceneStateRequestMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendSceneStateMessage_when_requested()
    {
        long requestTimestamp = 1723834815802;
        WebMessage<Dictionary<string, object>> sceneStateRequestMessage = new WebMessage<Dictionary<string, object>>()
        {
            type = WebMessageType.requestSceneState,
            payload = new Dictionary<string, object>()
            {
                { "requestTimestamp", requestTimestamp }
            }
        };
        editMediator.ReceiveWebMessage(JsonConvert.SerializeObject(sceneStateRequestMessage));
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.requestSceneStateSuccess, sentMessage.type);
        Assert.IsTrue(JToken.DeepEquals(mockModelInfoManager.GetSceneState(), sentMessage.payload["sceneState"]));
        Assert.AreEqual(requestTimestamp, sentMessage.payload["requestTimestamp"].ToObject<long>());
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldRejectRequestChangeMaterialMessageWithNoMaterialId()
    {
        Dictionary<string, object> messageWithNoMaterialId = new Dictionary<string, object>
        {
            { "type", WebMessageType.changeObjectsMaterial },
            { "payload", new Dictionary<string, object>
                {
                    { "material", new JObject()
                        {
                            ["texture"] = "material-1-url",
                            ["widthMillimeters"] = 2000,
                            ["heightMillimeters"] = 1500
                        }
                    },
                    {"objectIds", new string[] { "id-0", "id-1" } },
                }
            }
        };
        var serializedMessage = JsonConvert.SerializeObject(messageWithNoMaterialId);
        editMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearSelectionBeforeSendingSceneStateMessage()
    {
        WebMessage<Dictionary<string, object>> sceneStateRequestMessage = new WebMessage<Dictionary<string, object>>()
        {
            type = WebMessageType.requestSceneState,
            payload = new Dictionary<string, object>()
            {
                {"requestTimestamp", 1723834815802 }
            }
        };

        Assert.IsFalse(mockSelectedObjectsManager.selectionCleared);
        Assert.IsFalse(mockModelInfoManager.sceneStateRequested);

        string serializedMessage = JsonConvert.SerializeObject(sceneStateRequestMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        Assert.IsTrue(mockSelectedObjectsManager.selectionCleared);
        Assert.IsFalse(mockModelInfoManager.sceneStateRequested);
        yield return null;
        Assert.IsTrue(mockSelectedObjectsManager.selectionCleared);
        Assert.IsTrue(mockModelInfoManager.sceneStateRequested);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldPaintObjects_when_Receive_loadSceneRequest_with_onlyOneColor()
    {
        string color = "#00FF00";
        Color expectedColor = Color.green;
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", color },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-3" },
                    { "color", color },
                    { "material", null }
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        Assert.AreEqual(new string[] { "object-id-1", "object-id-3" }, registrySpy.lastRequestedObjectIds);
        Assert.AreEqual(expectedColor, changeColorManagerSpy.lastCalledColor);
        Assert.AreEqual(registrySpy.objects, changeColorManagerSpy.lastCalledObjects);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
    }

    [UnityTest]
    public IEnumerator ShouldPaintObjects_when_Receive_loadSceneRequest_with_severalColors()
    {
        string greenColor = "#00FF00";
        Color expectedGreenColor = Color.green;
        string redColor = "#FF0000";
        Color expectedRedColor = Color.red;
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", greenColor },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", redColor },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-3" },
                    { "color", greenColor },
                    { "material", null }
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        List<string[]> expectedObjectIdsGroups = new List<string[]>
        {
            new string[] { "object-id-1", "object-id-3" },
            new string[] { "object-id-2" },
        };
        List<Color> expectedColors = new List<Color>
        {
            expectedGreenColor,
            expectedRedColor,
        };
        Assert.AreEqual(expectedObjectIdsGroups, registrySpy.requestedObjectIds);
        Assert.AreEqual(expectedColors, changeColorManagerSpy.calledColors);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
    }

    [UnityTest]
    public IEnumerator ShouldClearSelectedObjects()
    {
        Assert.AreEqual(mockSelectedObjectsManager.selectionCleared, false);
        string message = JObject.FromObject(new { type = WebMessageType.clearSelectedObjects }).ToString();
        editMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.AreEqual(mockSelectedObjectsManager.selectionCleared, true);
    }

    [Test]
    public void ShouldRequestChangeObjectsMaterial_when_Receive_loadSceneRequest_with_onlyOneMaterial()
    {
        JObject material = new JObject()
        {
            { "id", 123456 },
            { "texture", "material-1-url" },
            { "widthMillimeters", 2000 },
            { "heightMillimeters", 1500}
        };
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", null },
                    { "material", material }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-3" },
                    { "color", null },
                    { "material", material }
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        JObject expectedMaterialChangeRequest = new JObject
        {
            { "material", material },
            { "objectIds", new JArray(new string[] { "object-id-1", "object-id-3" }) },
        };
        AssertExpectedData(expectedMaterialChangeRequest, changeMaterialControllerSpy.lastReceivedMessageRequest);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
    }

    [Test]
    public void ShouldRequestChangeObjectsMaterial_when_Receive_loadSceneRequest_with_severalMaterials()
    {
        JObject material1 = new JObject()
        {
            { "id", 123456 },
            { "texture", "material-1-url" },
            { "widthMillimeters", 2000 },
            { "heightMillimeters", 1500}
        };
        JObject material2 = new JObject()
        {
            { "id", 1234567 },
            { "texture", "material-2-url" },
            { "widthMillimeters", 20000 },
            { "heightMillimeters", 15000 }
        };
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", null },
                    { "material", material1 }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material", material2 }
                },
                new JObject()
                {
                    { "objectId", "object-id-3" },
                    { "color", null },
                    { "material", material1 }
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);

        JObject[] expectedMaterialChangeRequests = new JObject[]
        {
            new JObject
            {
                { "material", material1 },
                { "object_ids", new JArray(new string[] { "object-id-1", "object-id-3" }) },
            },
            new JObject
            {
                { "material", material2 },
                { "object_ids", new JArray(new string[] { "object-id-2" }) },
            },
        };
        AssertExpectedData(expectedMaterialChangeRequests, changeMaterialControllerSpy.receivedMessageRequests);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
    }

    [Test]
    public void ShouldSendLoadSceneSuccessMessage_after_loadingSceneObjects()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", "#FF0000" },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material",
                        new JObject()
                        {
                            { "id", 12345 },
                            { "texture", "material-1-url" },
                            { "widthMillimeters", 2000 },
                            { "heightMillimeters", 1500}
                        }
                    }
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
        Assert.AreEqual(WebMessageType.requestSceneLoadSuccess, sentMessage.type);
        Assert.AreEqual(sentMessage.payload["requestTimestamp"].ToObject<long>(), 1723834815802);
    }

    [Test]
    public void ShouldNotPaintOrRequestMaterialChange_when_objectHaveColorAndMaterialIdSetToNull()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "objectId", "object-id-1" },
                    { "color", null },
                    { "material", null }
                },
                new JObject()
                {
                    { "objectId", "object-id-2" },
                    { "color", null },
                    { "material", null }

                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editMediator.ReceiveWebMessage(serializedMessage);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
        Assert.AreEqual(0, changeColorManagerSpy.calledColors.Count);
        Assert.AreEqual(0, changeMaterialControllerSpy.receivedMessageRequests.Count);
    }

    [Test]
    public void ShouldRequestOnceForOriginalSceneToBeRestored_when_Receive_loadSceneRequest()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray();
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        Assert.AreEqual(0, originalSceneControllerSpy.restoreOriginalSceneCallsCount);
        editMediator.ReceiveWebMessage(serializedMessage);
        Assert.AreEqual(1, originalSceneControllerSpy.restoreOriginalSceneCallsCount);
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_When_ShowObjectsMessageWithoutObjectIds()
    {
        JObject message = new JObject
        {
            { "type", WebMessageType.showObjects },
            { "payload", new JObject()
                {
                    { "requestId", "UUID" }
                }
            }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_When_ShowObjectsMessageWithoutRequestId()
    {
        JObject message = new JObject
        {
            { "type", WebMessageType.showObjects },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(new string[] { "id-0", "id-1" }) }
                }
            }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_When_HideObjectsMessageWithoutObjectIds()
    {
        JObject message = new JObject
        {
            { "type", WebMessageType.hideObjects },
            { "payload", new JObject()
                {
                    { "requestId", "UUID" }
                }
            }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_When_HideObjectsMessageWithoutRequestId()
    {
        JObject message = new JObject
        {
            { "type", WebMessageType.showObjects },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(new string[] { "id-0", "id-1" }) }
                }
            }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_When_ShowAllObjectsMessageWithoutRequestId()
    {
        JObject message = new JObject
        {
            { "type", WebMessageType.showAllObjects },
            { "payload", new JObject() }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string[]> sentMessage = (WebMessage<string[]>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
    }

    [UnityTest]
    public IEnumerator ShouldSendSuccessMessage_When_ValidShowObjectsMessage()
    {
        const string requestId = "UUID";
        string[] objectIds = new string[] { "id-0", "id-1" };
        JObject message = new JObject
        {
            { "type", WebMessageType.showObjects },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(objectIds) },
                    { "requestId", requestId }
                }
            }
        };

        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.showHideObjectsSuccess, sentMessage.type);
        Assert.AreEqual(buildingVisibilityControllerSpy.lastRequestedVisibility, true);
        Assert.AreEqual(objectIds, buildingVisibilityControllerSpy.lastRequestedObjectIds);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
    }

    [UnityTest]
    public IEnumerator ShouldSendFailureMessage_When_ShowObjectsFails()
    {
        const string requestId = "UUID";
        string[] objectIds = new string[] { "id-0", "id-1" };
        const string errorMessage = "Error, Fail to change visibility from spy";
        buildingVisibilityControllerSpy.error = true;
        JObject message = new JObject
        {
            { "type", WebMessageType.showObjects },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(objectIds) },
                    { "requestId", requestId }
                }
            }
        };

        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.showHideObjectsFailure, sentMessage.type);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
        Assert.AreEqual(errorMessage, sentMessage.payload.Value<string>("errorMessage"));
    }

    [UnityTest]
    public IEnumerator ShouldSendSuccessMessage_When_ValidHideObjectsMessage()
    {
        const string requestId = "UUID";
        string[] objectIds = new string[] { "id-0", "id-1" };
        JObject message = new JObject
        {
            { "type", WebMessageType.hideObjects },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(objectIds) },
                    { "requestId", requestId }
                }
            }
        };

        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.showHideObjectsSuccess, sentMessage.type);
        Assert.AreEqual(buildingVisibilityControllerSpy.lastRequestedVisibility, false);
        Assert.AreEqual(objectIds, buildingVisibilityControllerSpy.lastRequestedObjectIds);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
    }

    [UnityTest]
    public IEnumerator ShouldSendFailureMessage_When_HideObjectsFails()
    {
        const string requestId = "UUID";
        string[] objectIds = new string[] { "id-0", "id-1" };
        const string errorMessage = "Error, Fail to change visibility from spy";
        buildingVisibilityControllerSpy.error = true;
        JObject message = new JObject
        {
            { "type", WebMessageType.hideObjects },
            { "payload", new JObject()
                {
                    { "objectIds", new JArray(objectIds) },
                    { "requestId", requestId }
                }
            }
        };

        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.showHideObjectsFailure, sentMessage.type);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
        Assert.AreEqual(errorMessage, sentMessage.payload.Value<string>("errorMessage"));
    }

    [UnityTest]
    public IEnumerator ShouldSendSuccessMessage_When_ValidShowAllObjectsMessage()
    {
        const string requestId = "UUID";
        JObject message = new JObject
        {
            { "type", WebMessageType.showAllObjects },
            { "payload", new JObject()
                {
                    { "requestId", requestId }
                }
            }
        };

        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.showHideObjectsSuccess, sentMessage.type);
        Assert.AreEqual(null, buildingVisibilityControllerSpy.lastRequestedObjectIds);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
    }

    [UnityTest]
    public IEnumerator ShouldSendFailureMessage_When_ShowAllObjectsFails()
    {
        const string requestId = "UUID";
        const string errorMessage = "Error, Fail to change visibility from spy";
        buildingVisibilityControllerSpy.error = true;
        JObject message = new JObject
        {
            { "type", WebMessageType.showAllObjects },
            { "payload", new JObject()
                {
                    { "requestId", requestId }
                }
            }
        };

        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.showHideObjectsFailure, sentMessage.type);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
        Assert.AreEqual(errorMessage, sentMessage.payload.Value<string>("errorMessage"));
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessage_when_ActivateViewModeFailure()
    {
        const string requestId = "UUID";
        const ViewMode viewMode = ViewMode.dollhouse;
        viewModeManagerSpy.throwError = true;
        JObject message = new JObject
        {
            { "type", WebMessageType.activateViewMode },
            { "payload", new JObject()
                {
                    { "requestId", requestId },
                    { "viewMode", viewMode.ToString() }
                }
            }
        };
        editMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.activateViewModeFailure, sentMessage.type);
        Assert.AreEqual(requestId, sentMessage.payload.Value<string>("requestId"));
        Assert.AreEqual(viewModeManagerSpy.errorMessage, sentMessage.payload.Value<string>("errorMessage"));
    }

}
