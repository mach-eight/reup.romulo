using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

using ReupVirtualTwin.helpers;
using Newtonsoft.Json.Linq;


public class DataValidatorTest : MonoBehaviour
{

    JObject parentSchema;
    JObject nestedObjectSchema;
    JObject nestedNestedObjectschema;

    [SetUp]
    public void Setup()
    {
        nestedNestedObjectschema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties",  new JObject
                {
                    { "nested_nested_string", new JObject
                        {
                            { "type", DataValidator.stringType }
                        }
                    },
                    { "nested_nested_int", new JObject
                        {
                            { "type", DataValidator.intType }
                        }
                    }
                }
            }
        };
        nestedObjectSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties",  new JObject
                {
                    { "nested_string", new JObject
                        {
                            { "type", DataValidator.stringType }
                        }
                    },
                    { "nested_int", new JObject
                        {
                            { "type", DataValidator.intType }
                        }
                    },
                }
            }
        };
        parentSchema = new JObject
        {
            { "type", DataValidator.objectType },
            { "properties", new JObject()
                {
                    { "a_string", new JObject
                        {
                            { "type", DataValidator.stringType }
                        }
                    },
                    { "an_int", new JObject
                        {
                            { "type", DataValidator.intType }
                        }
                    },
                }
            }
        };
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

    //[Test]
    //public void ShouldValidateArrays()
    //{
    //    Dictionary<string, object> schemaKeys = new Dictionary<string, object>
    //    {
    //        { "int_array", new Dictionary<string, object> 
    //            {
    //            { "s", JTokenType.Ara }
    //            }
    //        },
    //        { "age", JTokenType.Integer },
    //    };
    //}

}
