using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using ReupVirtualTwin.helpers;

using Newtonsoft.Json.Linq;
using ReupVirtualTwin.enums;

public class IncomingMessageValidatorTest
{
    IncomingMessageValidator incomingMessageValidator;

    [SetUp]
    public void SetUp()
    {
        incomingMessageValidator = new IncomingMessageValidator();
    }

    [UnityTest]
    public IEnumerator ShouldNotValidateMessageWithoutTypeField()
    {
        JObject incomingMessage = new JObject()
        {
            ["noTypeField"] = "test"
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, errors.Count);
        Assert.AreEqual("Incoming message does not contain a type field", errors[0]);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldNotValidateMessageWithUnsupportedType()
    {
        string unsupportedType = "unsupportedType";
        JObject incomingMessage = new JObject()
        {
            ["type"] = unsupportedType
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, errors.Count);
        Assert.AreEqual($"Incoming message type '{unsupportedType}' is not supported", errors[0]);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldValidateMessageWithSupportedTypeAndNoPayloadWhenNoPayloadIsExpected()
    {
        string supportedType = WebMessageType.activatePositionTransform;
        JObject incomingMessage = new JObject()
        {
            ["type"] = supportedType
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, errors.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldNotValidateMessageWithSupportedTypeAndNotExpectedPayload()
    {
        string supportedType = WebMessageType.activatePositionTransform;
        JObject incomingMessage = new JObject()
        {
            ["type"] = supportedType,
            ["payload"] = "UnexpectedPayload"
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, errors.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldNotValidateMessageWithInvalidPayload()
    {
        string supportedType = WebMessageType.setEditMode;
        JObject incomingMessage = new JObject()
        {
            ["type"] = supportedType,
            ["payload"] = "invalidPayload"
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, errors.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldValidateMessageWithValidPayload()
    {
        string supportedType = WebMessageType.setEditMode;
        JObject incomingMessage = new JObject()
        {
            ["type"] = supportedType,
            ["payload"] = true
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsTrue(isValid);
        Assert.AreEqual(0, errors.Count);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ShouldNotValidateMessageWithNullPayloadWhenPayloadIsExpected()
    {
        string supportedType = WebMessageType.setEditMode;
        JObject incomingMessage = new JObject()
        {
            ["type"] = supportedType,
        };
        IList<string> errors;
        bool isValid = incomingMessageValidator.ValidateMessage(incomingMessage, out errors);
        Assert.IsFalse(isValid);
        Assert.AreEqual(1, errors.Count);
        yield return null;
    }       

}
