using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Tests.PlayMode.Mocks;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using Newtonsoft.Json.Linq;

public class EditionMediatorTest : MonoBehaviour
{
    GameObject containerGameObject;
    EditionMediator editionMediator;
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

    [SetUp]
    public void SetUp()
    {
        containerGameObject = new GameObject();
        editionMediator = containerGameObject.AddComponent<EditionMediator>();
        mockEditModeManager = new MockEditModeManager();
        mockSelectedObjectsManager = new MockSelectedObjectsManager();
        editionMediator.editModeManager = mockEditModeManager;
        editionMediator.selectedObjectsManager = mockSelectedObjectsManager;
        mockWebMessageSender = new MockWebMessageSender();
        editionMediator.webMessageSender = mockWebMessageSender;
        mockTransformObjectsManager = new MockTransformObjectsManager();
        editionMediator.transformObjectsManager = mockTransformObjectsManager;
        mockInsertObjectsManager = new MockInsertObjectsManager(editionMediator);
        editionMediator.insertObjectsController = mockInsertObjectsManager;
        mockObjectMapper = new MockObjectMapper();
        editionMediator.objectMapper = mockObjectMapper;
        registrySpy = new ObjectRegistrySpy();
        editionMediator.registry = registrySpy;
        changeColorManagerSpy = new ChangeColorManagerSpy();
        editionMediator.changeColorManager = changeColorManagerSpy;
        changeMaterialControllerSpy = new ChangeMaterialControllerSpy();
        editionMediator.changeMaterialController = changeMaterialControllerSpy;
        mockModelInfoManager = new MockModelInfoManager();
        editionMediator.modelInfoManager = mockModelInfoManager;
        requestSceneLoadMessage = new JObject(
            new JProperty("type", WebMessageType.requestSceneLoad),
            new JProperty("payload", new JObject()
            {
                { "scene_name", "scene-name" },
                { "scene_id", 12345 },
                { "objects",  null },
            })
        );
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
        public Task ChangeObjectMaterial(JObject message, bool notify)
        {
            receivedMessageRequests.Add(message);
            lastReceivedMessageRequest = message;
            return Task.CompletedTask;
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
        private bool _allowSelection = false;
        public bool allowSelection { get => _allowSelection; set => _allowSelection = value; }


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
                tags = new Tag[2] { new Tag() { id = "tag0" }, new Tag() { id = "tag1" } },
            };
        }

        public WebMessage<ModelInfoMessage> ObtainModelInfoMessage()
        {
            WebMessage<ModelInfoMessage> message = new()
            {
                type = WebMessageType.requestModelInfoSuccess,
                payload = new ModelInfoMessage()
                {
                    buildVersion = "2024-04-05",
                    building = building,
                },
            };
            return message;
        }

        public WebMessage<UpdateBuildingMessage> ObtainUpdateBuildingMessage()
        {
            WebMessage<UpdateBuildingMessage> message = new()
            {
                type = WebMessageType.updateBuilding,
                payload = new UpdateBuildingMessage()
                {
                    building = building,
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
        private IMediator editionMediator;
        public MockInsertObjectsManager(IMediator mediator)
        {
            calledToInsertObject = false;
            objectLoadString = null;
            editionMediator = mediator;
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
            editionMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
            requestedObjectId = insertObjectMessagePayload.objectId;
        }
    }
    private class MockObjectMapper : IObjectMapper
    {
        public ObjectDTO[] objectDTOs = new ObjectDTO[2]
        {
            new ObjectDTO
            {
                id = "id0",
                tags = new Tag[2]{ new Tag() { id = "tag0"},  new Tag() { id = "tag1"} },
            },
            new ObjectDTO
            {
                id = "id1",
                tags = new Tag[2]{ new Tag() { id = "tag2"},  new Tag() { id = "tag3"} },
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
        editionMediator.Notify(ReupEvent.setEditMode, true);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(true, sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToFalse()
    {
        editionMediator.Notify(ReupEvent.setEditMode, false);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(false, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSetEditModeWhenReceiveRequest()
    {
        string message = JObject.FromObject(new { type = WebMessageType.setEditMode, payload = true }).ToString();
        editionMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, true);
        yield return null;
        message = JObject.FromObject(new { type = WebMessageType.setEditMode, payload = false }).ToString();
        editionMediator.ReceiveWebMessage(message);
        Assert.AreEqual(mockEditModeManager.editMode, false);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldAllowAndDisallowObjectSelection()
    {
        editionMediator.Notify(ReupEvent.setEditMode, true);
        Assert.AreEqual(mockSelectedObjectsManager.allowSelection, true);
        yield return null;
        editionMediator.Notify(ReupEvent.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.allowSelection, false);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldClearSelectionWhenEditModeIsSetToFalse()
    {
        editionMediator.Notify(ReupEvent.setEditMode, false);
        Assert.AreEqual(mockSelectedObjectsManager.selectionCleared, true);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendErrorMessageIfReceibeMessageToActivatePositionTransformModeWithNoSelectedObject()
    {
        string message = dummyJsonCreator.createWebMessage(WebMessageType.activatePositionTransform);
        editionMediator.ReceiveWebMessage(message);
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
        editionMediator.ReceiveWebMessage(message);
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
        editionMediator.ReceiveWebMessage(message);
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
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        Assert.IsTrue(mockInsertObjectsManager.calledToInsertObject);
        Assert.AreEqual(payload.objectUrl, mockInsertObjectsManager.objectLoadString);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectUpdatedProcess()
    {
        float processStatus = 0.25f;
        editionMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        WebMessage<float> sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
        processStatus = 0.6f;
        editionMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessages[1];
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectSuccessAndUpdateBuildingMessage()
    {
        InsertedObjectPayload insertedObjectPayload = new InsertedObjectPayload()
        {
            loadedObject = new GameObject("insertedObject"),
        };
        editionMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        yield return null;
        WebMessage<ObjectDTO> sentMessage = (WebMessage<ObjectDTO>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.loadObjectSuccess, sentMessage.type);
        Assert.AreEqual(mockObjectMapper.objectDTOs[0], sentMessage.payload);
        WebMessage<UpdateBuildingMessage> sentUpdateBuildingMessage = (WebMessage<UpdateBuildingMessage>)mockWebMessageSender.sentMessages[1];  
        Assert.AreEqual(WebMessageType.updateBuilding, sentUpdateBuildingMessage.type);
        Assert.AreEqual(mockModelInfoManager.building, sentUpdateBuildingMessage.payload.building);
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
        editionMediator.ReceiveWebMessage(message);
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
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editionMediator.noInsertObjectUrlErrorMessage, sentMessage.payload);
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
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editionMediator.noInsertObjectIdErrorMessage, sentMessage.payload);
        yield return null;
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
        editionMediator.ReceiveWebMessage(message);
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
        editionMediator.ReceiveWebMessage(message);
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
            objectIds = new string[] {"id-0", "id-1"},
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.changeObjectColor, payload);
        editionMediator.ReceiveWebMessage(message);
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
            objectIds = new string[] {"id-0", "id-1"},
        };
        string message = dummyJsonCreator.createWebMessage(WebMessageType.changeObjectColor, payload);
        editionMediator.ReceiveWebMessage(message);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        Assert.AreEqual(editionMediator.InvalidColorErrorMessage(color), sentMessage.payload);
    }

    [UnityTest]
    public IEnumerator ShouldRequestChangeMaterialOfObjects_When_ReceivesChangeMaterialRequest()
    {

        Dictionary<string, object> message = new Dictionary<string, object>
        {
            { "type", WebMessageType.changeObjectsMaterial },
            { "payload", new Dictionary<string, object>
                {
                    {"material_id", 1234},
                    {"material_url", "material-url"},
                    {"object_ids", new string[] { "id-0", "id-1" } },
                }
            }
        };
        var serializedMessage = JsonConvert.SerializeObject(message);
        editionMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        AssertExpectedData(message["payload"], changeMaterialControllerSpy.lastReceivedMessageRequest);
    }


    [UnityTest]
    public IEnumerator ShouldSendSuccessMessage_When_NotifiedOfMaterialChangeSuccess()
    {
        JObject materialChangeInfo = new JObject(
            new JProperty("material_id", 1234),
            new JProperty("material_url", "material-url"),
            new JProperty("object_ids", new JArray(new string[] { "id-0", "id-1" }))
        );
        editionMediator.Notify(ReupEvent.objectMaterialChanged, materialChangeInfo);
        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.changeObjectsMaterialSuccess, sentMessage.type);
        Assert.AreEqual(materialChangeInfo["material_url"], sentMessage.payload["material_url"]);
        Assert.AreEqual(materialChangeInfo["object_ids"], sentMessage.payload["object_ids"]);
        yield return null;
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
        editionMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
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
        editionMediator.ReceiveWebMessage(message.ToString());
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendDeleteObjectSuccessAndUpdateBuildingMessage()
    {
        editionMediator.Notify(ReupEvent.objectsDeleted);
        WebMessage<string> deletedSuccessMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        yield return null;
        Assert.AreEqual(WebMessageType.deleteObjectsSuccess, deletedSuccessMessage.type);
        WebMessage<UpdateBuildingMessage> sentUpdateBuildingMessage = (WebMessage<UpdateBuildingMessage>)mockWebMessageSender.sentMessages[1];
        Assert.AreEqual(WebMessageType.updateBuilding, sentUpdateBuildingMessage.type);
        Assert.AreEqual(mockModelInfoManager.building, sentUpdateBuildingMessage.payload.building);
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
        editionMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendSceneStateMessage_when_requested()
    {
        string sceneName = "the-scene-name";
        WebMessage<Dictionary<string, object>> sceneStateRequestMessage = new WebMessage<Dictionary<string, object>>()
        {
            type = WebMessageType.requestSceneState,
            payload = new Dictionary<string, object>()
            {
                {"scene_name", sceneName }
            }
        };
        editionMediator.ReceiveWebMessage(JsonConvert.SerializeObject(sceneStateRequestMessage));
        yield return null;

        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(WebMessageType.requestSceneStateSuccess, sentMessage.type);
        Assert.IsTrue(JToken.DeepEquals(mockModelInfoManager.GetSceneState(), sentMessage.payload["scene_state"]));
        Assert.AreEqual(sceneName, sentMessage.payload["scene_name"].ToString());
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
                    {"material_url", "material-url"},
                    {"object_ids", new string[] { "id-0", "id-1" } },
                }
            }
        };
        var serializedMessage = JsonConvert.SerializeObject(messageWithNoMaterialId);
        editionMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessages[0];
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
                {"scene_name", "test-scene-name" }
            }
        };

        Assert.IsFalse(mockSelectedObjectsManager.selectionCleared);
        Assert.IsFalse(mockModelInfoManager.sceneStateRequested);

        string serializedMessage = JsonConvert.SerializeObject(sceneStateRequestMessage);
        editionMediator.ReceiveWebMessage(serializedMessage);
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
                    { "object_id", "object-id-1" },
                    { "color", color },
                },
                new JObject()
                {
                    { "object_id", "object-id-2" },
                },
                new JObject()
                {
                    { "object_id", "object-id-3" },
                    { "color", color },
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editionMediator.ReceiveWebMessage(serializedMessage);
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
                    { "object_id", "object-id-1" },
                    { "color", greenColor },
                },
                new JObject()
                {
                    { "object_id", "object-id-2" },
                    { "color", redColor },
                },
                new JObject()
                {
                    { "object_id", "object-id-3" },
                    { "color", greenColor },
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        editionMediator.ReceiveWebMessage(serializedMessage);
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

    [Test]
    public async Task ShouldRequestChangeObjectsMaterial_when_Receive_loadSceneRequest_with_onlyOneMaterial()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "object_id", "object-id-1" },
                    { "material_id", 1 },
                    { "material_url", "material-1-url" },
                },
                new JObject()
                {
                    { "object_id", "object-id-2" },
                },
                new JObject()
                {
                    { "object_id", "object-id-3" },
                    { "material_id", 1 },
                    { "material_url", "material-1-url" },
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        await editionMediator.ReceiveWebMessage(serializedMessage);
        JObject expectedMaterialChangeRequest = new JObject
        {
            { "material_id", 1 },
            { "material_url", "material-1-url" },
            { "object_ids", new JArray(new string[] { "object-id-1", "object-id-3" }) },
        };
        AssertExpectedData(expectedMaterialChangeRequest, changeMaterialControllerSpy.lastReceivedMessageRequest);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
    }

    [Test]
    public async Task ShouldRequestChangeObjectsMaterial_when_Receive_loadSceneRequest_with_severalMaterials()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "object_id", "object-id-1" },
                    { "material_id", 1 },
                    { "material_url", "material-1-url" },
                },
                new JObject()
                {
                    { "object_id", "object-id-2" },
                    { "material_id", 2 },
                    { "material_url", "material-2-url" },
                },
                new JObject()
                {
                    { "object_id", "object-id-3" },
                    { "material_id", 1 },
                    { "material_url", "material-1-url" },
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        await editionMediator.ReceiveWebMessage(serializedMessage);
        JObject[] expectedMaterialChangeRequests = new JObject[]
        {
            new JObject
            {
                { "material_id", 1 },
                { "material_url", "material-1-url" },
                { "object_ids", new JArray(new string[] { "object-id-1", "object-id-3" }) },
            },
            new JObject
            {
                { "material_id", 2 },
                { "material_url", "material-2-url" },
                { "object_ids", new JArray(new string[] { "object-id-2" }) },
            },
        };
        AssertExpectedData(expectedMaterialChangeRequests, changeMaterialControllerSpy.receivedMessageRequests);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
    }

    [Test]
    public async Task ShouldSendLoadSceneSuccessMessage_after_loadingSceneObjects()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "object_id", "object-id-1" },
                    { "color", "#FF0000" },
                },
                new JObject()
                {
                    { "object_id", "object-id-2" },
                    { "material_id", 1 },
                    { "material_url", "material-1-url" },
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        await editionMediator.ReceiveWebMessage(serializedMessage);
        WebMessage<JObject> sentMessage = (WebMessage<JObject>)mockWebMessageSender.sentMessages[0];
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
        Assert.AreEqual(WebMessageType.requestSceneLoadSuccess, sentMessage.type);
        Assert.AreEqual("scene-name", sentMessage.payload["scene_name"].ToString());
        Assert.AreEqual(12345, sentMessage.payload["scene_id"].ToObject<int>());
    }

    [Test]
    public async Task ShouldNotPaintOrRequestMaterialChange_when_objectHaveColorAndMaterialIdSetToNull()
    {
        requestSceneLoadMessage["payload"]["objects"] = new JArray(
            new JObject[]
            {
                new JObject()
                {
                    { "object_id", "object-id-1" },
                    { "color", null },
                    { "material_id", null },
                    { "material_url", null },
                },
                new JObject()
                {
                    { "object_id", "object-id-2" },
                    { "color", null },
                    { "material_id", null },
                    { "material_url", null },
                },
            }
        );
        string serializedMessage = JsonConvert.SerializeObject(requestSceneLoadMessage);
        await editionMediator.ReceiveWebMessage(serializedMessage);
        Assert.AreEqual(1, mockWebMessageSender.sentMessages.Count);
        Assert.AreEqual(0, changeColorManagerSpy.calledColors.Count);
        Assert.AreEqual(0, changeMaterialControllerSpy.receivedMessageRequests.Count);
    }

}
