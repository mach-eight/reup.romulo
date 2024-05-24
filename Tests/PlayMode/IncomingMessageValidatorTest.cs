using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

using ReupVirtualTwin.helpers;
using Newtonsoft.Json.Linq;

public class IncomingMessageValidatorTest
{
    IncomingMessageValidator incomingMessageValidator;
    JObject invalidTypeMessage;
    JObject messageA;
    JObject messageAPayloadSchema;
    JObject messageAWithIncorrectPayload;

    [SetUp]
    public void SetUp()
    {
        incomingMessageValidator = new IncomingMessageValidator();
        invalidTypeMessage = new JObject
        {
            { "type", "this is an invalid type" },
        };
        messageA = new JObject
        {
            { "type", "messageA" },
            { "payload", true }
        };
        messageAPayloadSchema = DataValidator.boolSchema;
        messageAWithIncorrectPayload = new JObject
        {
            { "type", "messageA" },
            { "payload", "I am supposed to be a boolean, not a string" }
        };
    }

    [Test]
    public void IncomingMessageValidatorExists()
    {
        Assert.IsNotNull(incomingMessageValidator);
    }

    [Test]
    public void ShouldFailAllMessageByDefault()
    {
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(messageA.ToString()));
    }

    [Test]
    public void ShouldFailIncorrectJson()
    {
        Assert.IsFalse(incomingMessageValidator.ValidateMessage("\"this is not a valid message json\""));
    }

    [Test]
    public void ShouldApproveMessage_if_typeAndPayloadSchemaIsRegistered()
    {
        incomingMessageValidator.RegisterMessage((string)messageA["type"], messageAPayloadSchema);
        Assert.IsTrue(incomingMessageValidator.ValidateMessage(messageA.ToString()));
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(invalidTypeMessage.ToString()));
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(messageAWithIncorrectPayload.ToString()));
    }

}
