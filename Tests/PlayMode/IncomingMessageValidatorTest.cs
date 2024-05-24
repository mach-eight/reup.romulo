using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

using ReupVirtualTwin.helpers;
using Newtonsoft.Json.Linq;

public class IncomingMessageValidatorTest
{
    string messageWithBoolPayloadType;
    IncomingMessageValidator incomingMessageValidator;
    JObject invalidTypeMessage;
    JObject messageWithBoolPayload;
    JObject messageWithIncorrectPayload;
    JObject messageWithNoPayload;

    [SetUp]
    public void SetUp()
    {
        messageWithBoolPayloadType = "message with bool payload";
        incomingMessageValidator = new IncomingMessageValidator();
        invalidTypeMessage = new JObject
        {
            { "type", "this is an invalid type" },
        };
        messageWithBoolPayload = new JObject
        {
            { "type", messageWithBoolPayloadType },
            { "payload", true }
        };
        messageWithIncorrectPayload = new JObject
        {
            { "type", messageWithBoolPayloadType },
            { "payload", "I am supposed to be a boolean, not a string" }
        };
        messageWithNoPayload = new JObject
        {
            { "type", "message with no payload" }
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
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(messageWithBoolPayload.ToString()));
    }

    [Test]
    public void ShouldFailIncorrectJson()
    {
        Assert.IsFalse(incomingMessageValidator.ValidateMessage("\"this is not a valid message json\""));
    }

    [Test]
    public void ShouldApproveMessage_if_typeAndPayloadSchemaIsRegistered()
    {
        incomingMessageValidator.RegisterMessage((string)messageWithBoolPayload["type"], DataValidator.boolSchema);
        Assert.IsTrue(incomingMessageValidator.ValidateMessage(messageWithBoolPayload.ToString()));
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(invalidTypeMessage.ToString()));
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(messageWithIncorrectPayload.ToString()));
    }

    [Test]
    public void ShouldApproveMessageWithNoPayload()
    {
        Assert.IsFalse(incomingMessageValidator.ValidateMessage(messageWithNoPayload.ToString()));
        incomingMessageValidator.RegisterMessage((string)messageWithNoPayload["type"]);
        Assert.IsTrue(incomingMessageValidator.ValidateMessage(messageWithNoPayload.ToString()));
    }

}
