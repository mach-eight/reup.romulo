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

        [TearDown]
        public void TearDown()
        {
            ReupSceneInstantiator.DestroySceneObjects(sceneObjects);
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

    }

}