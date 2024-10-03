using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using ReupVirtualTwin.dataSchemas;
using ReupVirtualTwin.enums;
using System.Collections.Generic;

namespace ReupVirtualTwin.helpers
{
    public class IncomingMessageValidator
    {
        private static Dictionary<string, JSchema> payloadJSchemaForMessageType = new Dictionary<string, JSchema>
        {
            { WebMessageType.activatePositionTransform, null },
            { WebMessageType.activateRotationTransform, null },
            { WebMessageType.deactivateTransformMode, null },
            { WebMessageType.requestModelInfo, null },
            { WebMessageType.clearSelectedObjects, null },
            { WebMessageType.allowSelection, null },
            { WebMessageType.disableSelection, null },
            { WebMessageType.setEditMode, BasicTypesSchemas.boolSchema },
            { WebMessageType.deleteObjects, BasicTypesSchemas.stringSchema },
            { WebMessageType.loadObject, BasicTypesSchemas.stringSchema },
            { WebMessageType.changeObjectColor, BasicTypesSchemas.stringSchema },
            { WebMessageType.changeObjectsMaterial, RomuloExternalSchema.changeObjectMaterialPayloadSchema },
            { WebMessageType.requestSceneState, RomuloExternalSchema.requestSceneStatePayloadSchema },
            { WebMessageType.requestSceneLoad, RomuloExternalSchema.requestLoadScenePayloadSchema },
            { WebMessageType.activateDHV, null },
            { WebMessageType.activateFPV, null },
            { WebMessageType.showObjects, RomuloExternalSchema.showOrHideObjectsPayloadSchema },
            { WebMessageType.hideObjects, RomuloExternalSchema.showOrHideObjectsPayloadSchema },
            { WebMessageType.showAllObjects, RomuloExternalSchema.showAllObjectsPayloadSchema },
        };

        public bool ValidateMessage(JObject incomingMessage, out IList<string> errorMessages)
        {
            errorMessages = new List<string>();
            if (!incomingMessage.ContainsKey("type"))
            {
                errorMessages.Add("Incoming message does not contain a type field");
                return false;
            }
            string messageType = incomingMessage["type"].ToString();
            if (!payloadJSchemaForMessageType.ContainsKey(messageType))
            {
                errorMessages.Add($"Incoming message type '{messageType}' is not supported");
                return false;
            }

            JToken payload = incomingMessage["payload"];
            JSchema schema = payloadJSchemaForMessageType[messageType];

            return ValidateMessagePayload(payload, schema, out errorMessages);
        }

        private bool ValidateMessagePayload(JToken payload, JSchema schema, out IList<string> errorMessages)
        {
            errorMessages = new List<string>();
            if (schema == null)
            {
                if (payload != null)
                {
                    errorMessages.Add("Incoming message should not contain a payload");
                    return false;
                }
                return true;
            };

            if (payload == null)
            {
                errorMessages.Add("Incoming message should contain a payload");
                return false;
            }

            return payload.IsValid(schema, out errorMessages);
        }
    }
}