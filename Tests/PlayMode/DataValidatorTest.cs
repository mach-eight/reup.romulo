using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

using ReupVirtualTwin.helpers;
using Newtonsoft.Json.Linq;


public class DataValidatorTest
{
    JObject parentSchema;
    JObject nestedObjectSchema;
    JObject nestedNestedObjectschema;
    JObject intStringArraySchema;

    [SetUp]
    public void Setup()
    {
        parentSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject()
                {
                    { "a_string", DataValidator.stringSchema},
                    { "an_int", DataValidator.intSchema},
                }
            }
        };
        nestedObjectSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties",  new JObject
                {
                    { "nested_string", DataValidator.stringSchema },
                    { "nested_int", DataValidator.intSchema},
                }
            }
        };
        nestedNestedObjectschema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties",  new JObject
                {
                    { "nested_nested_string", DataValidator.stringSchema },
                    { "nested_nested_int", DataValidator.intSchema }
                }
            }
        };
        intStringArraySchema = DataValidator.CreateArraySchema(new JObject[] { DataValidator.intSchema, DataValidator.stringSchema });
    }

    [Test]
    public void ValidateStringAndIntSchema_should_success()
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "a_string", "John Doe" },
            { "an_int", 25 }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void Validation_should_fail_ifTypeIsWrong()
    {
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "a_string", "John Doe" },
            { "an_int", "25" }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void NestedValidation_should_success()
    {
        parentSchema["properties"]["nested_object"] = nestedObjectSchema;
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "a_string", "John Doe" },
            { "an_int", 25 },
            { "nested_object", new Dictionary<string, object>
                {
                    { "nested_string", "Jane Doe" },
                    { "nested_int", 30 }
                } }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void NestedValidation_should_fail_if_typeIsWrong()
    {
        parentSchema["properties"]["nested_object"] = nestedObjectSchema;
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "a_string", "John Doe" },
            { "an_int", 25 },
            { "nested_object", new Dictionary<string, object>
                {
                    { "nested_string", "Jane Doe" },
                    { "nested_int", "30" }
                } }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void SeveralNestedValidation_should_success()
    {
        nestedObjectSchema["properties"]["nested_nested_object"] = nestedNestedObjectschema;
        parentSchema["properties"]["nested_object"] = nestedObjectSchema;
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "a_string", "John Doe" },
            { "an_int", 25 },
            { "nested_object", new Dictionary<string, object>
                {
                    { "nested_string", "Jane Doe" },
                    { "nested_int", 30 },
                    { "nested_nested_object", new Dictionary<string, object>()
                        {
                            { "nested_nested_string", "Jimmy Doe" },
                            { "nested_nested_int", 8 }
                        }
                    }
                } }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void SeveralNestedValidation_should_fail_ifWrongNestedType()
    {
        nestedObjectSchema["properties"]["nested_nested_object"] = nestedNestedObjectschema;
        parentSchema["properties"]["nested_object"] = nestedObjectSchema;
        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "a_string", "John Doe" },
            { "an_int", 25 },
            { "nested_object", new Dictionary<string, object>
                {
                    { "nested_string", "Jane Doe" },
                    { "nested_int", 30 },
                    { "nested_nested_object", new Dictionary<string, object>()
                        {
                            { "nested_nested_string", 5 },
                            { "nested_nested_int", 8 }
                        }
                    }
                } }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void ValidateArraysShould_success()
    {
        parentSchema["properties"]["nested_array"] = intStringArraySchema;
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "a_string", "John Doe" },
            { "an_int", 25 },
            { "nested_array", new object[] { "Jane Doe", 1234, "this is a string int mixed array" } }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void ValidateArraysShould_fail_if_wrongItemsInArray()
    {
        parentSchema["properties"]["nested_array"] = intStringArraySchema;
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            { "a_string", "John Doe" },
            { "an_int", 25 },
            { "nested_array", new object[] { "Jane Doe", 1234, "this is a string int mixed array", false } }
        };
        bool result = DataValidator.ValidateObjectToSchema(data, parentSchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void ValidateString_should_success()
    {
        bool result = DataValidator.ValidateObjectToSchema("John Doe", DataValidator.stringSchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void ValidateString_should_fail_ifWrongTypeIsGiven()
    {
        bool result = DataValidator.ValidateObjectToSchema(5, DataValidator.stringSchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void ValidateBool_should_success()
    {
        bool result = DataValidator.ValidateObjectToSchema(true, DataValidator.boolSchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void ValidateBool_should_fail_ifWrongTypeIsGiven()
    {
        bool result = DataValidator.ValidateObjectToSchema("this is not a boolean", DataValidator.boolSchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void ValidateArray_should_success()
    {
        bool result = DataValidator.ValidateObjectToSchema(new string[] { "1", "2" }, intStringArraySchema);
        Assert.IsTrue(result);
    }

    [Test]
    public void ValidateArray_should_fail_ifWrongTypeIsGiven()
    {
        bool result = DataValidator.ValidateObjectToSchema("this is not an array", intStringArraySchema);
        Assert.IsFalse(result);
    }

    [Test]
    public void ValidateObject_should_fail_ifWrongTypeIsGiven()
    {
        bool result = DataValidator.ValidateObjectToSchema("this is not an object", parentSchema);
        Assert.IsFalse(result);
    }

}