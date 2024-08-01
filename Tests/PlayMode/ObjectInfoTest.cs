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
        ObjectInfo objectInfo;

        [SetUp]
        public void SetUp()
        {
            testObj = new GameObject("testObj");
            objectInfo = testObj.AddComponent<ObjectInfo>();
        }

        [TearDown]
        public void TearDown()
        {
            Destroy(testObj);
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
            GameObject testObjWithMaterial = new GameObject("testObjWithMaterial");
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
