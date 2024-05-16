using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;
using System;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.helpers
{
    public class DataManipulationHelpersTest
    {

        DummyDataStructure dummyDataStructure;
        string serializedData;
        Dictionary<string, object> deserializedData;

        [SetUp]
        public void SetUp()
        {
            dummyDataStructure = new()
            {
                myString = "1",
                myInt = 1,
                myIntArray = new int[] { 1, 2, 3 },
                myFloat = 1.5f,
                myFloatArray = new float[] { 1.5f, 2.5f, 3.5f },
                myStringArray = new string[] { "1", "2", "3" },
                myNest = new()
                {
                    myString = "2",
                    myInt = 2,
                    myIntArray = new int[] { 1, 2, 3 },
                    myFloat = 1.5f,
                    myFloatArray = new float[] { 1.5f, 2.5f, 3.5f },
                    myStringArray = new string[] { "1", "2", "3" },
                    myNestNest = new()
                    {
                        myString = "3",
                        myInt = 3,
                        myIntArray = new int[] { 1, 2, 3 },
                        myFloat = 1.5f,
                        myFloatArray = new float[] { 1.5f, 2.5f, 3.5f },
                        myStringArray = new string[] { "1", "2", "3" },
                    }
                }
            };
            serializedData = JsonConvert.SerializeObject(dummyDataStructure);
            Debug.Log("serializedDataStructure");
            Debug.Log(serializedData);
            deserializedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedData);
        }

        private class DummyDataStructure
        {
            public string myString;
            public int myInt;
            public float myFloat;
            public int[] myIntArray;
            public float[] myFloatArray;
            public string[] myStringArray;
            public NestedDummyDataStructure myNest;
            public class NestedDummyDataStructure
            {
                public string myString;
                public int myInt;
                public float myFloat;
                public int[] myIntArray;
                public float[] myFloatArray;
                public string[] myStringArray;
                public NestedNestedDummyDataStructure myNestNest;

                public class NestedNestedDummyDataStructure
                {
                    public string myString;
                    public int myInt;
                    public float myFloat;
                    public int[] myIntArray;
                    public float[] myFloatArray;
                    public string[] myStringArray;
                }
            }

        }

        [Test]
        public void ShouldReadFieldAndReturnWithCorrectTypes()
        { 
            var myString = deserializedData.GetValueAtPath(new string[] { "myString" });
            var myInt = deserializedData.GetValueAtPath(new string[] { "myInt" });
            var myIntArray = deserializedData.GetValueAtPath(new string[] { "myIntArray" });
            var myFloat = deserializedData.GetValueAtPath(new string[] { "myFloat" });
            var myFloatArray = deserializedData.GetValueAtPath(new string[] { "myFloatArray" });
            var myStringArray = deserializedData.GetValueAtPath(new string[] { "myStringArray" });
            Assert.IsTrue(myString is string);
            Assert.IsTrue(myInt is Int64); // I don't understand why this is Int64 instead of normal Int32
            Assert.IsTrue(myIntArray is int[]);
            Assert.IsTrue(myFloat is double); // I don't understand why this is double instead of float
            Assert.IsTrue(myFloatArray is float[]);
            Assert.IsTrue(myStringArray is string[]);

        }


        [Test]
        public void ShouldReadFieldFromDeserializedObject()
        {
            var myString = deserializedData.GetValueAtPath(new string[] { "myString" });
            var myInt = deserializedData.GetValueAtPath(new string[] { "myInt" });
            var myIntArray = deserializedData.GetValueAtPath(new string[] { "myIntArray" });
            var myFloat = deserializedData.GetValueAtPath(new string[] { "myFloat" });
            var myFloatArray = deserializedData.GetValueAtPath(new string[] { "myFloatArray" });
            var myStringArray = deserializedData.GetValueAtPath(new string[] { "myStringArray" });
            Assert.AreEqual(dummyDataStructure.myString, myString);
            Assert.AreEqual(dummyDataStructure.myInt, myInt);
            Assert.AreEqual(dummyDataStructure.myIntArray, myIntArray);
            Assert.AreEqual(dummyDataStructure.myFloat, myFloat);
            Assert.AreEqual(dummyDataStructure.myFloatArray, myFloatArray);
            Assert.AreEqual(dummyDataStructure.myStringArray, myStringArray);
        }

        [Test]
        public void ShouldReadNestedFieldFromDeserializedObject()
        {
            var myNestNestString = deserializedData.GetValueAtPath(new string[] { "myNest", "myNestNest", "myString" });
            var myNestNestInt = deserializedData.GetValueAtPath(new string[] { "myNest", "myNestNest", "myInt" });
            var myNestNestIntArray = deserializedData.GetValueAtPath(new string[] { "myNest", "myNestNest", "myIntArray" });
            var myNestNestFloat = deserializedData.GetValueAtPath(new string[] { "myNest", "myNestNest", "myFloat" });
            var myNestNestFloatArray = deserializedData.GetValueAtPath(new string[] { "myNest", "myNestNest", "myFloatArray" });
            var myNestNestStringArray = deserializedData.GetValueAtPath(new string[] { "myNest", "myNestNest", "myStringArray" });
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myString, myNestNestString);
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myInt, myNestNestInt);
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myIntArray, myNestNestIntArray);
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myFloat, myNestNestFloat);
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myFloatArray, myNestNestFloatArray);
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myStringArray, myNestNestStringArray);
        }

        [Test]
        public void ShouldReadFieldFromReadObject()
        {
            var myNest = deserializedData.GetValueAtPath(new string[] { "myNest" });
            var myNestNest = ((Dictionary<string, object>)myNest).GetValueAtPath(new string[] { "myNestNest" });
            var myNestNestStringArray = ((Dictionary<string, object>)myNestNest).GetValueAtPath(new string[] { "myStringArray" });
            Assert.AreEqual(dummyDataStructure.myNest.myNestNest.myStringArray, myNestNestStringArray);
        }

    }
}
