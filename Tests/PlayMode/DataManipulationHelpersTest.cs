using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwinTests.helpers
{
    public class DataManipulationHelpersTest
    {

        DummyDataStructure dummyDataStructure;
        string serializedDataStructure;

        [SetUp]
        public void SetUp()
        {
            dummyDataStructure = new()
            {
                myString = "1",
                myInt = 1,
                myNest = new()
                {
                    myString = "2",
                    myInt = 2,
                    myNestNest = new()
                    {
                        myString = "3",
                        myInt = 3
                    }
                }
            };
            serializedDataStructure = JsonConvert.SerializeObject(dummyDataStructure);
            Debug.Log("serializedDataStructure");
            Debug.Log(serializedDataStructure);
        }

        private class DummyDataStructure
        {
            public string myString;
            public int myInt;
            public NestedDummyDataStructure myNest;
            public class NestedDummyDataStructure
            {
                public string myString;
                public int myInt;
                public NestedNestedDummyDataStructure myNestNest;

                public class NestedNestedDummyDataStructure
                {
                    public string myString;
                    public int myInt;
                }
            }

        }

        [Test]
        public void ShouldReadFieldFromDeserializedObject()
        {
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedDataStructure);
            Assert.AreEqual(
                dummyDataStructure.myString,
                DataManipulationHelpers.GetValueAtPath(data, new string[] {"myString"})
            );
            Assert.AreEqual(
                dummyDataStructure.myInt,
                DataManipulationHelpers.GetValueAtPath(data, new string[] {"myInt"})
            );
        }

        [Test]
        public void ShouldReadNestedFieldFromDeserializedObject()
        {
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedDataStructure);
            Assert.AreEqual(
                dummyDataStructure.myNest.myNestNest.myString,
                DataManipulationHelpers.GetValueAtPath(data, new string[] { "myNest", "myNestNest", "myString" })
            );
            Assert.AreEqual(
                dummyDataStructure.myNest.myNestNest.myInt,
                DataManipulationHelpers.GetValueAtPath(data, new string[] { "myNest", "myNestNest", "myInt" })
            );
        }

        [Test]
        public void ShouldReadObjectFieldFromDeserializedObject()
        {
            Dictionary<string, object> data = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedDataStructure);
            Dictionary<string, object> expected = new()
            {
                { "myString", dummyDataStructure.myNest.myNestNest.myString },
                { "myInt", dummyDataStructure.myNest.myNestNest.myInt }
            };
            Assert.AreEqual(
                expected,
                DataManipulationHelpers.GetValueAtPath(data, new string[] { "myNest", "myNestNest" })
            );
        }

    }
}
