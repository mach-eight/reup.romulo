using UnityEngine;
using System;
using System.Collections.Generic;

using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.romuloEnvironment;
using ReupVirtualTwin.dataSchemas;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Schema;
using Zenject;

namespace ReupVirtualTwin.managers
{
    public class EditMediator : MonoBehaviour, IMediator, IWebMessageReceiver
    {
        private ICharacterRotationManager characterRotationManager;
        private IEditModeManager _editModeManager;
        public IEditModeManager editModeManager { set => _editModeManager = value; }
        private ISelectedObjectsManager _selectedObjectsManager;
        public ISelectedObjectsManager selectedObjectsManager { set { _selectedObjectsManager = value; } }
        private ITransformObjectsManager _transformObjectsManager;
        public ITransformObjectsManager transformObjectsManager { set => _transformObjectsManager = value; }
        private IDeleteObjectsManager _deleteObjectsManager;
        public IDeleteObjectsManager deleteObjectsManager { set => _deleteObjectsManager = value; }

        private IChangeColorManager _changeColorManager;
        public IChangeColorManager changeColorManager { set => _changeColorManager = value; }

        private IInsertObjectsController _insertObjectsController;
        public IInsertObjectsController insertObjectsController { set => _insertObjectsController = value; }

        private IWebMessagesSender _webMessageSender;
        public IWebMessagesSender webMessageSender { set { _webMessageSender = value; } }
        private IObjectMapper _objectMapper;
        public IObjectMapper objectMapper { set => _objectMapper = value; }

        private IObjectRegistry _registry;
        public IObjectRegistry registry { set => _registry = value; get => _registry; }

        private IChangeMaterialController _changeMaterialController;
        public IChangeMaterialController changeMaterialController
        {
            get => _changeMaterialController; set => _changeMaterialController = value;
        }

        private IViewModeManager _viewModeManager;
        public IViewModeManager viewModeManager { set => _viewModeManager = value; get => _viewModeManager; }

        private IncomingMessageValidator incomingMessageValidator = new IncomingMessageValidator();

        [HideInInspector]
        public string noInsertObjectIdErrorMessage = "No object id provided for insertion";
        [HideInInspector]
        public string noInsertObjectUrlErrorMessage = "No 3d model url provided for insertion";

        public string InvalidColorErrorMessage(string colorCode) => $"Invalid color code {colorCode}";

        private IModelInfoManager _modelInfoManager;
        public IModelInfoManager modelInfoManager { set => _modelInfoManager = value; }

        private IOriginalSceneController _originalSceneController;
        public IOriginalSceneController originalSceneController { get => _originalSceneController; set => _originalSceneController = value; }
        public ISpacesRecord spacesRecord { get; set; }
        public IBuildingVisibilityController buildingVisibilityController { get; set; }

        ICharacterPositionManager characterPositionManager;
        ITagsController tagsController;

        [Inject]
        public void Init(
            ICharacterPositionManager characterPositionManager,
            ICharacterRotationManager characterRotationManager,
            ITagsController tagsController)
        {
            this.characterRotationManager = characterRotationManager;
            this.characterPositionManager = characterPositionManager;
            this.tagsController = tagsController;
        }

        public void Notify(ReupEvent eventName)
        {
            switch (eventName)
            {
                case ReupEvent.transformHandleStartInteraction:
                    characterRotationManager.allowRotation = false;
                    break;
                case ReupEvent.transformHandleStopInteraction:
                    characterRotationManager.allowRotation = true;
                    break;
                case ReupEvent.positionTransformModeActivated:
                    ProcessTransformModeActivation(TransformMode.PositionMode);
                    break;
                case ReupEvent.rotationTransformModeActivated:
                    ProcessTransformModeActivation(TransformMode.RotationMode);
                    break;
                case ReupEvent.transformModeDeactivated:
                    ProcessTranformModeDeactivation();
                    break;
                case ReupEvent.objectsDeleted:
                    StartCoroutine(ProcessDeletedObjects());
                    break;
                case ReupEvent.objectColorChanged:
                    ProcessObjectColorChanged();
                    break;
                default:
                    throw new ArgumentException($"no implementation without payload for event: {eventName}");
            }
        }

        public void Notify<T>(ReupEvent eventName, T payload)
        {
            switch (eventName)
            {
                case ReupEvent.setEditMode:
                    if (!(payload is bool))
                    {
                        throw new ArgumentException($"Payload must be a boolean for {eventName} events", nameof(payload));
                    }
                    ProccessEditMode((bool)(object)payload);
                    break;
                case ReupEvent.setSelectedObjects:
                    if (!(payload is ObjectWrapperDTO))
                    {
                        throw new ArgumentException($"Payload must be of type {nameof(ObjectWrapperDTO)} for {eventName} events", nameof(payload));
                    }
                    ProcessNewWrapper((ObjectWrapperDTO)(object)payload);
                    break;
                case ReupEvent.insertedObjectLoaded:
                    if (!(payload is InsertedObjectPayload))
                    {
                        throw new ArgumentException($"Payload must be of type {nameof(InsertedObjectPayload)} for {eventName} events", nameof(payload));
                    }
                    StartCoroutine(ProcessInsertedObjectLoaded((InsertedObjectPayload)(object)payload));
                    break;
                case ReupEvent.insertedObjectStatusUpdate:
                    if (!(payload is float))
                    {
                        throw new ArgumentException($"Payload must be of type float for {eventName} events", nameof(payload));
                    }
                    ProcessLoadStatus((float)(object)payload);
                    break;
                case ReupEvent.objectMaterialChanged:
                    if (RomuloEnvironment.development)
                    {
                        if (!((JObject)(object)payload).IsValid(RomuloExternalSchema.changeObjectMaterialPayloadSchema))
                        {
                            Debug.LogWarning("Invalid payload for objectMaterialChanged event");
                            return;
                        }
                    }
                    ProcessObjectMaterialsChange((JObject)(object)payload);
                    break;
                case ReupEvent.spaceJumpPointReached:
                    if (RomuloEnvironment.development && !((JObject)(object)payload).IsValid(RomuloInternalSchema.spaceJumpInfoEventPayload))
                    {
                        throw new ArgumentException($"Invalid payload for '{eventName}' event");
                    }
                    ProcessSpaceJumpPointReached((JObject)(object)payload);
                    break;
                case ReupEvent.spaceJumpPointWithNoGround:
                    if (RomuloEnvironment.development && !((JObject)(object)payload).IsValid(RomuloInternalSchema.spaceJumpInfoEventPayload))
                    {
                        throw new ArgumentException($"Invalid payload for '{eventName}' event");
                    }
                    ProcessSpaceJumpPointWithNoGround((JObject)(object)payload);
                    break;
                case ReupEvent.spaceJumpPointNotFound:
                    if (RomuloEnvironment.development && !((JObject)(object)payload).IsValid(RomuloInternalSchema.spaceJumpInfoEventPayload))
                    {
                        throw new ArgumentException($"Invalid payload for '{eventName}' event");
                    }
                    ProcessSpaceJumpPointNotFound((JObject)(object)payload);
                    break;
                case ReupEvent.slideToSpacePointRequestInterrupted:
                    if (RomuloEnvironment.development && !((JObject)(object)payload).IsValid(RomuloInternalSchema.spaceJumpInfoEventPayload))
                    {
                        throw new ArgumentException($"Invalid payload for '{eventName}' event");
                    }
                    ProcessJumpToSpacePointInterrupted((JObject)(object)payload);
                    break;
                case ReupEvent.error:
                    if (!(payload is string))
                    {
                        throw new ArgumentException($"Payload must be of type string for {eventName} events", nameof(payload));
                    }
                    SendErrorMessage((string)(object)payload);
                    break;
                default:
                    throw new ArgumentException($"no implementation for event: {eventName}");
            }
        }

        public void ReceiveWebMessage(string serializedWebMessage)
        {
            JObject message = JObject.Parse(serializedWebMessage);
            IList<string> errorMessages;
            if (!incomingMessageValidator.ValidateMessage(message, out errorMessages))
            {
                foreach (string error in errorMessages)
                {
                    Debug.LogWarning(error);
                }
                SendErrorMessages(errorMessages.ToArray());
                return;
            }
            string type = message["type"].ToString();
            JToken payload = message["payload"];
            _ = ReceiveWebMessage(type, payload);
        }

        public async Task ReceiveWebMessage(string type, JToken payload)
        {
            try
            {
                await ExecuteWebMessage(type, payload);
            }
            catch (Exception e)
            {
                Debug.LogError("An error ocurred why executing the WebMessage");
                Debug.LogError(e.Message);
                Debug.LogError(e);
            }
        }
        public async Task ExecuteWebMessage(string type, JToken payload)
        {
            switch (type)
            {
                case WebMessageType.setEditMode:
                    _editModeManager.editMode = ((JValue)payload).Value<bool>();
                    break;
                case WebMessageType.activatePositionTransform:
                    ActivateTransformMode(TransformMode.PositionMode);
                    break;
                case WebMessageType.activateRotationTransform:
                    ActivateTransformMode(TransformMode.RotationMode);
                    break;
                case WebMessageType.deactivateTransformMode:
                    DeactivateTransformMode();
                    break;
                case WebMessageType.deleteObjects:
                    DeleteSelectedObjects(payload.ToString());
                    break;
                case WebMessageType.loadObject:
                    LoadObject(payload.ToString());
                    break;
                case WebMessageType.changeObjectColor:
                    ChangeObjectsColor(payload.ToString());
                    break;
                case WebMessageType.requestModelInfo:
                    SendModelInfoMessage();
                    break;
                case WebMessageType.changeObjectsMaterial:
                    TaskResult result = await _changeMaterialController.ChangeObjectMaterial((JObject)payload);
                    if (!result.isSuccess)
                    {
                        SendObjectMaterialsChangeFailure(result.error);
                        return;
                    }
                    ProcessObjectMaterialsChange((JObject)payload);
                    break;
                case WebMessageType.requestSceneState:
                    StartCoroutine(SendSceneStateMessage((JObject)payload));
                    break;
                case WebMessageType.requestSceneLoad:
                    await LoadObjectsState((JObject)payload);
                    break;
                case WebMessageType.clearSelectedObjects:
                    _selectedObjectsManager.ClearSelection();
                    break;
                case WebMessageType.allowSelection:
                    _selectedObjectsManager.allowEditSelection = true;
                    break;
                case WebMessageType.disableSelection:
                    _selectedObjectsManager.allowEditSelection = false;
                    break;
                case WebMessageType.activateViewMode:
                    ActivateViewMode((JObject)payload);
                    break;
                case WebMessageType.slideToSpace:
                    spacesRecord.GoToSpace((JObject)payload);
                    break;
                case WebMessageType.showObjects:
                    SetObjectsVisibility((JObject)payload, true);
                    break;
                case WebMessageType.hideObjects:
                    SetObjectsVisibility((JObject)payload, false);
                    break;
                case WebMessageType.showAllObjects:
                    ShowAllObjects((JObject)payload);
                    break;
                case WebMessageType.requestObjectTagsUnderCharacter:
                    ProcessObjectTagsUnderCharacterRequest(payload["requestId"].ToString());
                    break;
            }
        }

        void ProcessObjectTagsUnderCharacterRequest(string requestId)
        {
            Vector3 characterPosition = characterPositionManager.characterPosition;
            LayerMask buildingLayerMask = LayerMaskUtils.GetLayerMaskById(RomuloLayerIds.buildingLayerId);
            Ray characterDownRay = new Ray(characterPosition, Vector3.down);
            RaycastHit hit;
            if (Physics.Raycast(characterDownRay, out hit, Mathf.Infinity, buildingLayerMask))
            {
                GameObject hitObject = hit.collider.gameObject;
                SendObjectTagsUnderCharacterResponse(requestId, hitObject);
                return;
            }
            SendFailureMessage(requestId, "No object under character", WebMessageType.requestObjectTagsUnderCharacterFailure);
        }

        void SendObjectTagsUnderCharacterResponse(string requestId, GameObject obj)
        {
            List<Tag> tags = tagsController.GetTagsFromObject(obj);
            WebMessage<JObject> message = new()
            {
                type = WebMessageType.requestObjectTagsUnderCharacterSuccess,
                payload = new JObject(
                    new JProperty("requestId", requestId),
                    new JProperty("tags", JArray.FromObject(tags))
                )
            };
            _webMessageSender.SendWebMessage(message);
        }

        void ActivateViewMode(JObject payload)
        {
            try
            {
                string viewMode = payload["viewMode"].ToString();
                if (viewMode == ViewMode.dollhouse.ToString())
                {
                    _viewModeManager.ActivateDHV();
                }
                else if (viewMode == ViewMode.firstPerson.ToString())
                {
                    _viewModeManager.ActivateFPV();
                }
                SendActivateViewModeSuccessMessage(payload);
            }
            catch (Exception e)
            {
                SendFailureMessage(payload["requestId"].ToString(), e.Message, WebMessageType.activateViewModeFailure);
            }
        }

        private void SetObjectsVisibility(JObject payload, bool show)
        {
            string[] objectIds = payload["objectIds"].ToObject<string[]>();
            TaskResult isSuccess = buildingVisibilityController.SetObjectsVisibility(objectIds, show);
            if (!isSuccess.isSuccess)
            {
                SendFailureMessage(payload["requestId"].ToString(), isSuccess.error, WebMessageType.showHideObjectsFailure);
                return;
            }
            SendHideShowObjectsSuccessMessage(payload);
        }

        private void ShowAllObjects(JObject payload)
        {
            TaskResult isSuccess = buildingVisibilityController.ShowAllObjects();
            if (!isSuccess.isSuccess)
            {
                SendFailureMessage(payload["requestId"].ToString(), isSuccess.error, WebMessageType.showHideObjectsFailure);
                return;
            }
            SendHideShowObjectsSuccessMessage(payload);
        }

        private void SendHideShowObjectsSuccessMessage(JObject payload)
        {
            WebMessage<JObject> message = new()
            {
                type = WebMessageType.showHideObjectsSuccess,
                payload = new JObject(
                    new JProperty("requestId", payload["requestId"])
                )
            };
            _webMessageSender.SendWebMessage(message);
        }

        private async Task LoadObjectsState(JObject requestPayload)
        {
            originalSceneController.RestoreOriginalScene();
            List<JObject> objectStates = requestPayload["objects"].ToObject<JArray>().Cast<JObject>().ToList();

            var objectStatesByColor = objectStates
                .Where(objectState => TypeHelpers.NotNull(objectState["color"]))
                .GroupBy(objectState => objectState["color"].ToString());
            TaskResult colorWasChanged = PaintSceneObjects(objectStatesByColor);

            var objectStatesByMaterial = objectStates
                .Where(objectState =>
                {
                    return TypeHelpers.NotNull(objectState["material"]) &&
                        TypeHelpers.NotNull(objectState["material"]["id"]) &&
                        TypeHelpers.NotNull(objectState["material"]["texture"]);
                })
                .GroupBy(objectState => objectState["material"]["id"].ToObject<int>());
            TaskResult materialWasChanged = await ApplyMaterialsToSceneObjects(objectStatesByMaterial);

            TaskResult finalResult = TaskResult.CombineResults(materialWasChanged, colorWasChanged);
            if (!finalResult.isSuccess)
            {
                originalSceneController.RestoreOriginalScene();
                SendLoadSceneFailureMessage(requestPayload, finalResult.error);
                return;
            }

            SendSuccessLoadSceneMessage(requestPayload);
        }

        private void SendSuccessLoadSceneMessage(JObject requestPayload)
        {
            WebMessage<JObject> successMessage = new()
            {
                type = WebMessageType.requestSceneLoadSuccess,
                payload = new JObject(
                    new JProperty("requestTimestamp", requestPayload["requestTimestamp"])
                )
            };
            _webMessageSender.SendWebMessage(successMessage);
        }

        private void SendLoadSceneFailureMessage(JObject requestPayload, string errorMessage)
        {

            WebMessage<JObject> failureMessage = new()
            {
                type = WebMessageType.requestSceneLoadFailure,
                payload = new JObject(
                   new JProperty("requestTimestamp", requestPayload["requestTimestamp"]),
                   new JProperty("errorMessage", errorMessage)
               )
            };
            _webMessageSender.SendWebMessage(failureMessage);
        }


        private TaskResult PaintSceneObjects(IEnumerable<IGrouping<string, JObject>> objectStatesByColor)
        {
            foreach (var objectsByColor in objectStatesByColor)
            {
                Color? color = Utils.ParseColor(objectsByColor.Key);
                if (color == null)
                {
                    return TaskResult.Failure(InvalidColorErrorMessage(objectsByColor.Key));
                }
                string[] objectIds = objectsByColor.Select(objectState => objectState["objectId"].ToString()).ToArray();
                List<GameObject> objectsToPaint = _registry.GetObjectsWithGuids(objectIds);
                _changeColorManager.ChangeObjectsColor(objectsToPaint, (Color)color);
            }
            return TaskResult.Success();
        }

        private async Task<TaskResult> ApplyMaterialsToSceneObjects(IEnumerable<IGrouping<int, JObject>> objectStatesByMaterial)
        {

            foreach (var objectsByMaterial in objectStatesByMaterial)
            {
                var objectIds = objectsByMaterial.Select(objectState => objectState["objectId"].ToString());
                JObject materialChangeInfo = new()
                {
                    { "material", objectsByMaterial.First()["material"] },
                    { "objectIds", new JArray(objectIds) },
                };
                TaskResult isSuccess = await _changeMaterialController.ChangeObjectMaterial(materialChangeInfo);
                if (!isSuccess.isSuccess)
                {
                    return isSuccess;
                }
            }
            return TaskResult.Success();
        }

        private IEnumerator SendSceneStateMessage(JObject sceneStateRequestPayload)
        {
            _selectedObjectsManager.ClearSelection();
            yield return null;
            JObject sceneState = ((ISceneStateManager)_modelInfoManager).GetSceneState();
            WebMessage<JObject> sceneStateMessage = new WebMessage<JObject>
            {
                type = WebMessageType.requestSceneStateSuccess,
                payload = new JObject(
                    new JProperty("sceneState", sceneState),
                    new JProperty("requestTimestamp", sceneStateRequestPayload["requestTimestamp"])),
            };
            _webMessageSender.SendWebMessage(sceneStateMessage);
        }

        public void SendModelInfoMessage()
        {
            WebMessage<JObject> message = _modelInfoManager.ObtainModelInfoMessage();
            _webMessageSender.SendWebMessage(message);
        }

        private void ActivateTransformMode(TransformMode mode)
        {
            if (_selectedObjectsManager.wrapperDTO == null || _selectedObjectsManager.wrapperDTO.wrapper == null)
            {
                SendErrorMessage($"Can't activate {mode} Transform mode because no object is selected");
                return;
            }
            _transformObjectsManager.ActivateTransformMode(_selectedObjectsManager.wrapperDTO, mode);
        }

        private void DeactivateTransformMode()
        {
            if (!_transformObjectsManager.active)
            {
                SendErrorMessage("Can't deactivate transform mode if no transform mode is currently active");
                return;
            }
            _transformObjectsManager.DeactivateTransformMode();
        }

        private void DeleteSelectedObjects(string stringIds)
        {
            List<GameObject> objectsToDelete = _deleteObjectsManager.GetDeletableObjects(stringIds);
            if (objectsToDelete.Count > 0)
            {
                foreach (GameObject obj in objectsToDelete)
                {
                    _selectedObjectsManager.ForceRemoveObjectFromSelection(obj);
                }
                _deleteObjectsManager.DeleteObjects(objectsToDelete);
            }
            else
            {
                SendErrorMessage("The selection is empty, or there is at least one non-deletable object selected");
            }

        }

        private void ChangeObjectsColor(string payload)
        {
            ChangeColorObjectMessagePayload parsedPayload = JsonUtility.FromJson<ChangeColorObjectMessagePayload>(payload);
            List<GameObject> objectsToChangeColor = _registry.GetObjectsWithGuids(parsedPayload.objectIds);
            if (objectsToChangeColor.Count > 0)
            {
                Color? parsedColor = Utils.ParseColor(parsedPayload.color);
                if (parsedColor != null)
                {
                    _changeColorManager.ChangeObjectsColor(objectsToChangeColor, (Color)parsedColor);
                    Notify(ReupEvent.objectColorChanged);
                }
                else
                {
                    SendErrorMessage(InvalidColorErrorMessage(parsedPayload.color));
                }
            }
            else
            {
                SendErrorMessage("The selection is empty");
            }
        }

        private void ProccessEditMode(bool editMode)
        {
            _selectedObjectsManager.allowEditSelection = editMode;
            if (editMode == false)
            {
                _selectedObjectsManager.ClearSelection();
                if (_transformObjectsManager.active)
                    _transformObjectsManager.DeactivateTransformMode();
            }
            WebMessage<bool> message = new WebMessage<bool>
            {
                type = WebMessageType.setEditModeSuccess,
                payload = editMode,
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void ProcessNewWrapper(ObjectWrapperDTO wrappedObject)
        {
            if (_transformObjectsManager.active)
            {
                _transformObjectsManager.wrapper = wrappedObject;
            }
            SendNewSelectedObjectsMessage(wrappedObject.wrappedObjects);
        }

        private void SendNewSelectedObjectsMessage(List<GameObject> selectedObjects)
        {
            ObjectDTO[] objectDTOs = _objectMapper.MapObjectsToDTO(selectedObjects);
            WebMessage<ObjectDTO[]> message = new WebMessage<ObjectDTO[]>
            {
                type = WebMessageType.setSelectedObjects,
                payload = objectDTOs
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void ProcessTransformModeActivation(TransformMode mode)
        {
            string eventName;
            if (mode == TransformMode.PositionMode)
            {
                eventName = WebMessageType.activatePositionTransformSuccess;
            }
            else if (mode == TransformMode.RotationMode)
            {
                eventName = WebMessageType.activateRotationTransformSuccess;
            }
            else
            {
                throw new Exception($"unknown TransformMode {mode}");
            }
            WebMessage<string> message = new WebMessage<string>
            {
                type = eventName,
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void ProcessTranformModeDeactivation()
        {
            WebMessage<string> message = new WebMessage<string>
            {
                type = WebMessageType.deactivateTransformModeSuccess,
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void SendUpdatedBuildingMessage()
        {
            WebMessage<JObject> message = _modelInfoManager.ObtainUpdateBuildingMessage();
            _webMessageSender.SendWebMessage(message);
        }

        private IEnumerator ProcessDeletedObjects()
        {
            string webMessageType;
            webMessageType = WebMessageType.deleteObjectsSuccess;

            WebMessage<string> message = new WebMessage<string>
            {
                type = webMessageType,
            };
            _webMessageSender.SendWebMessage(message);
            yield return null;
            SendUpdatedBuildingMessage();
        }

        private void ProcessObjectColorChanged()
        {
            WebMessage<string> message = new WebMessage<string>
            {
                type = WebMessageType.changeObjectColorSuccess,
            };
            _webMessageSender.SendWebMessage(message);
        }

        private IEnumerator ProcessInsertedObjectLoaded(InsertedObjectPayload insertedObjectPayload)
        {
            SendInsertedObjectMessage(insertedObjectPayload.loadedObject);
            _selectedObjectsManager.ClearSelection();
            yield return null;
            SendUpdatedBuildingMessage();
            if (insertedObjectPayload.selectObjectAfterInsertion)
            {
                _selectedObjectsManager.AddObjectToSelection(insertedObjectPayload.loadedObject);
            }
        }

        private void SendInsertedObjectMessage(GameObject obj)
        {
            ObjectDTO objectDTO = _objectMapper.MapObjectToDTO(obj);
            WebMessage<ObjectDTO> message = new WebMessage<ObjectDTO>
            {
                type = WebMessageType.loadObjectSuccess,
                payload = objectDTO
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void LoadObject(string payload)
        {
            InsertObjectMessagePayload parsedPayload = JsonUtility.FromJson<InsertObjectMessagePayload>(payload);
            if (parsedPayload.objectUrl == null || parsedPayload.objectUrl == "")
            {
                SendErrorMessage(noInsertObjectUrlErrorMessage);
                return;
            }
            if (parsedPayload.objectId == null || parsedPayload.objectId == "")
            {
                SendErrorMessage(noInsertObjectIdErrorMessage);
                return;
            }
            _insertObjectsController.InsertObject(parsedPayload);
        }

        private void ProcessLoadStatus(float status)
        {
            WebMessage<float> message = new WebMessage<float>
            {
                type = WebMessageType.loadObjectProcessUpdate,
                payload = status
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void ProcessObjectMaterialsChange(JObject materialsChangedInfo)
        {
            WebMessage<JObject> message = new()
            {
                type = WebMessageType.changeObjectsMaterialSuccess,
                payload = materialsChangedInfo
            };
            _webMessageSender.SendWebMessage(message);
        }

        private void SendObjectMaterialsChangeFailure(string message)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = WebMessageType.changeObjectsMaterialFailure,
                payload = new JObject(
                    new JProperty("errorMessage", message)
                )
            });
        }

        private void SendErrorMessage(string message)
        {
            _webMessageSender.SendWebMessage(new WebMessage<string>
            {
                type = WebMessageType.error,
                payload = message,
            });
        }

        private void SendErrorMessages(string[] errorMessages)
        {

            _webMessageSender.SendWebMessage(new WebMessage<string[]>
            {
                type = WebMessageType.error,
                payload = errorMessages,
            });
        }
        void ProcessSpaceJumpPointReached(JObject spaceJumpPointPayload)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceSuccess,
                payload = spaceJumpPointPayload
            });
        }

        void ProcessSpaceJumpPointNotFound(JObject payload)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceFailure,
                payload = new JObject
                {
                    { "errorMessage", $"Space jump point with id '{payload["spaceId"]}' not found" },
                    { "requestId", payload["requestId"] },
                    { "spaceId", payload["spaceId"] }
                }
            });
        }
        void ProcessSpaceJumpPointWithNoGround(JObject payload)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceFailure,
                payload = new JObject
                {
                    { "errorMessage", $"Space jump point with id '{payload["spaceId"]}' has no ground below to jump to" },
                    { "requestId", payload["requestId"] },
                    { "spaceId", payload["spaceId"] }
                }
            });
        }
        void ProcessJumpToSpacePointInterrupted(JObject payload)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = WebMessageType.slideToSpaceInterrupted,
                payload = new JObject
                {
                    { "requestId", payload["requestId"] },
                    { "spaceId", payload["spaceId"] }
                }
            });
        }

        void SendActivateViewModeSuccessMessage(JObject payload)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = WebMessageType.activateViewModeSuccess,
                payload = payload,
            });
        }

        void SendFailureMessage(string requestId, string errorMessage, string messageType)
        {
            _webMessageSender.SendWebMessage(new WebMessage<JObject>
            {
                type = messageType,
                payload = new JObject(
                    new JProperty("requestId", requestId),
                    new JProperty("errorMessage", errorMessage)
                )
            });
        }
    }

}

