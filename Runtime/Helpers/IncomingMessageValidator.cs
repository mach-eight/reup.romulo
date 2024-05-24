using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public class IncomingMessageValidator
    {
        private List<RegisteredMessage> registeredMessages = new List<RegisteredMessage>();
        private JObject objectWithTypeSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject 
                {
                    { "type", DataValidator.stringSchema }
                }
            }
        };

        private class RegisteredMessage
        {
            public string type;
            public JObject schema;
        }
        public void RegisterMessage(string type)
        {
            registeredMessages.Add(new RegisteredMessage
            {
                type = type,
            });
        }
        public void RegisterMessage(string type, JObject schema)
        {
            registeredMessages.Add(new RegisteredMessage
            {
                type = type,
                schema = schema
            });
        }
        public bool ValidateMessage(string message)
        {
            Debug.Log("validating message: " + message);
            if (!DataValidator.ValidateJsonStringToSchema(message, objectWithTypeSchema))
            {
                Debug.LogWarning("Received message does not contain a type key");
                return false;
            }
            JObject messageObject = JObject.Parse(message);
            string messageType = (string)messageObject["type"];
            foreach (RegisteredMessage registeredMessage in registeredMessages)
            {
                if (messageType == registeredMessage.type)
                {
                    if (registeredMessage.schema == null)
                    {
                        return true;
                    }
                    return DataValidator.ValidateObjectToSchema(messageObject["payload"], registeredMessage.schema);
                }
            }
            Debug.LogWarning($"Message type {messageType} not registered: ");
            return false;
        }
    }
}
