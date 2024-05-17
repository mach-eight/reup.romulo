using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json.Schema;

using ReupVirtualTwin.helpers;


public class DataValidatorTest : MonoBehaviour
{
    [Test]
    public void ValidateSchema_should_success()
    {
        JSchema schema = JSchema.Parse(@"
        {
          '$schema': 'http://json-schema.org/draft-07/schema#',
          'type': 'object',
          'properties': {
            'name': {'type': 'string'},
            'age': {'type': 'integer', 'minimum': 0}
          },
          'required': ['name', 'age']
        }");

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "name", "John Doe" },
            { "age", 25 }
        };

        bool result = DataValidator.ValidateObjectToSchema(data, schema);

        Assert.IsTrue(result);

    }

    [Test]
    public void ValidateSchema_should_fail()
    {
        JSchema schema = JSchema.Parse(@"
        {
          '$schema': 'http://json-schema.org/draft-07/schema#',
          'type': 'object',
          'properties': {
            'name': {'enum': ['juan', 'pedro']},
            'age': {'type': 'integer', 'minimum': 30}
          },
          'required': ['name', 'age']
        }");

        Dictionary<string, object> data = new Dictionary<string, object>
        {
            { "name", "John Doe" },
            { "age", 25 }
        };

        bool result = DataValidator.ValidateObjectToSchema(data, schema);

        Assert.IsFalse(result);

    }
}
