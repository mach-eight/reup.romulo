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
    }

    private class ChangeColorManagerSpy : IChangeColorManager
    {
        public List<GameObject> calledObjects;
        public Color color;
        public void ChangeObjectsColor(List<GameObject> objectsToDelete, Color color)
        {
            calledObjects = objectsToDelete;
            this.color = color;
        }
    }

    private class ChangeMaterialControllerSpy : IChangeMaterialController
    {
        public JObject receivedMessagePayload;
        public Task ChangeObjectMaterial(JObject message)
        {
            receivedMessagePayload = message;
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

    private class MockWebMessageSender : IWebMessagesSender
    {
        public object sentMessage;
        public void SendWebMessage<T>(WebMessage<T> webMessage)
        {
            sentMessage = webMessage;
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
                deselectPreviousSelection = insertObjectMessagePayload.deselectPreviousSelection,
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

    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToTrue()
    {
        editionMediator.Notify(ReupEvent.setEditMode, true);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(true, sentMessage.payload);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldSendMessageInSetEditModeToFalse()
    {
        editionMediator.Notify(ReupEvent.setEditMode, false);
        WebMessage<bool> sentMessage = (WebMessage<bool>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.setEditModeSuccess, sentMessage.type);
        Assert.AreEqual(false, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSetEditModeWhenReceiveRequest()
    {
        string message = JObject.FromObject(new { type = WebMessageType.setEditMode, payload = true }).ToString();
        Debug.Log("message");
        Debug.Log(message);
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
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
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
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
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
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
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
        WebMessage<float> sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
        processStatus = 0.6f;
        editionMediator.Notify(ReupEvent.insertedObjectStatusUpdate, processStatus);
        sentMessage = (WebMessage<float>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectProcessUpdate, sentMessage.type);
        Assert.AreEqual(processStatus, sentMessage.payload);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldSendMessageOfLoadObjectSuccess()
    {
        InsertedObjectPayload insertedObjectPayload = new()
        {
            loadedObject = new GameObject("insertedObject"),
        };
        editionMediator.Notify(ReupEvent.insertedObjectLoaded, insertedObjectPayload);
        WebMessage<ObjectDTO> sentMessage = (WebMessage<ObjectDTO>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.loadObjectSuccess, sentMessage.type);
        Assert.AreEqual(mockObjectMapper.objectDTOs[0], sentMessage.payload);
        yield return null;
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
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
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
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
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
        Assert.AreEqual(expectedColor, changeColorManagerSpy.color);
        Assert.AreEqual(registrySpy.objects, changeColorManagerSpy.calledObjects);
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
        WebMessage<string> sentMessage = (WebMessage<string>)mockWebMessageSender.sentMessage;
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
                    {"material_url", "material-url"},
                    {"object_ids", new string[] { "id-0", "id-1" } },
                }
            }
        };
        var serializedMessage = JsonConvert.SerializeObject(message);
        editionMediator.ReceiveWebMessage(serializedMessage);
        yield return null;
        Assert.AreEqual(
            ((Dictionary<string,object>)message["payload"])["material_url"],
            changeMaterialControllerSpy.receivedMessagePayload["material_url"].ToString()
        );
        Assert.AreEqual(
            ((string[])((Dictionary<string,object>)message["payload"])["object_ids"])[0],
            changeMaterialControllerSpy.receivedMessagePayload["object_ids"][0].ToString()
        );
        Assert.AreEqual(
            ((string[])((Dictionary<string,object>)message["payload"])["object_ids"])[1],
            changeMaterialControllerSpy.receivedMessagePayload["object_ids"][1].ToString()
        );
    }

    [UnityTest]
    public IEnumerator ShouldSendSuccessMessage_When_NotifiedOfMaterialChangeSuccess()
    {
        string[] objectIds = new string[] { "id-0", "id-1" };
        Dictionary<string, object> payload = new()
        {
            { "material_url", "material-url" },
            { "object_ids", objectIds },
        };
        editionMediator.Notify(ReupEvent.objectMaterialChanged, payload);
        WebMessage<Dictionary<string, object>> sentMessage = (WebMessage<Dictionary<string, object>>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.changeObjectsMaterialSuccess, sentMessage.type);
        Assert.AreEqual(payload["material_url"], sentMessage.payload["material_url"]);
        Assert.AreEqual(payload["object_ids"], sentMessage.payload["object_ids"]);
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
        WebMessage<IList<string>> sentMessage = (WebMessage<IList<string>>)mockWebMessageSender.sentMessage;
        Assert.AreEqual(WebMessageType.error, sentMessage.type);
        yield return null;
    }

}
