using System.Runtime.CompilerServices;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;

using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwinTests.utils;
using NUnit.Framework;

namespace ReupVirtualTwinTests.managers
{
    public class TexturesManagerTest : MonoBehaviour
    {
        GameObject testCubesPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/testCubes.prefab");
        GameObject cube0;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        ITexturesManager texturesManager;

        [SetUp]
        public void SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(testCubesPrefab);
            texturesManager = sceneObjects.texturesManager;
            cube0 = sceneObjects.building.transform.Find("Cube0").gameObject;
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
            yield return null;
        }

        [Test]
        public void ShouldApplyMaterialWithTextureToObject()
        {
            Material originalMaterial = cube0.GetComponent<Renderer>().material;
            Texture2D newTexture = new Texture2D(2, 2);
            Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial.SetTexture("_BaseMap", newTexture);
            Assert.AreNotEqual(originalMaterial.GetTexture("_BaseMap"), newTexture);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial);
            Material appliedMaterial = cube0.GetComponent<Renderer>().material;
            Assert.AreEqual(appliedMaterial.GetTexture("_BaseMap"), newTexture);
        }

        [UnityTest]
        public IEnumerator ShouldDestroyTexture_if_replacedByTheOnlyObjectWhereWasBeingUsed()
        {
            Texture2D newTexture1 = new Texture2D(2, 2);
            Assert.IsFalse(newTexture1 == null);
            Material newMaterial1 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial1.SetTexture("_BaseMap", newTexture1);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial1);
            yield return null;

            Texture2D newTexture2 = new Texture2D(2, 2);
            Material newMaterial2 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial2.SetTexture("_BaseMap", newTexture2);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial2);
            yield return null;

            Material appliedMaterial = cube0.GetComponent<Renderer>().material;
            Assert.AreEqual(appliedMaterial.GetTexture("_BaseMap"), newTexture2);
            Assert.IsTrue(newTexture1 == null);
        }

    }

}