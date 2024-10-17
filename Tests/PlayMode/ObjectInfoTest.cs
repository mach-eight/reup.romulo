using System.Collections;
using NUnit.Framework;
using ReupVirtualTwin.models;
using ReupVirtualTwin.modelInterfaces;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;

namespace ReupVirtualTwinTests.models
{
    public class ObjectInfoTest : MonoBehaviour
    {
        GameObject testObj;
        GameObject testObjWithMaterial;
        ObjectInfo objectInfo;

        [SetUp]
        public void SetUp()
        {
            testObj = new GameObject("testObj");
            objectInfo = testObj.AddComponent<ObjectInfo>();
            testObjWithMaterial = new GameObject("testObjWithMaterial");
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(testObj);
            GameObject.DestroyImmediate(testObjWithMaterial);
        }

        [UnityTest]
        public IEnumerator ShouldNotAddOriginalMaterialToObjectInfoOnInit_if_objectHasNoMaterial()
        {
            Assert.IsNull(objectInfo.originalMaterial);
            yield return null;
        }
        [UnityTest]
        public IEnumerator ShouldAddOriginalMaterialToObjectInfoOnInit_if_objectHasMaterial()
        {
            Material dummyMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            dummyMaterial.name = "my dummy material";
            testObjWithMaterial.AddComponent<MeshRenderer>().sharedMaterial = dummyMaterial;
            ObjectInfo objectWithMaterialInfo = testObjWithMaterial.AddComponent<ObjectInfo>();
            yield return null;
            Assert.AreEqual(dummyMaterial.name, objectWithMaterialInfo.originalMaterial.name);
            yield return null;
        }
    }
}
