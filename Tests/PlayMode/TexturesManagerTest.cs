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
        GameObject cube1;
        ReupSceneInstantiator.SceneObjects sceneObjects;
        ITexturesManager texturesManager;

        [SetUp]
        public void SetUp()
        {
            sceneObjects = ReupSceneInstantiator.InstantiateSceneWithBuildingFromPrefab(testCubesPrefab);
            texturesManager = sceneObjects.texturesManager;
            cube0 = sceneObjects.building.transform.Find("Cube0").gameObject;
            cube1 = sceneObjects.building.transform.Find("Cube1").gameObject;
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
            Assert.IsTrue(newTexture1 == null);
        }

        [UnityTest]
        public IEnumerator ShouldNotDestroyTexture_if_textureIsStillBeingUsedInOtherObjects()
        {
            Texture2D newTexture1 = new Texture2D(2, 2);
            Assert.IsFalse(newTexture1 == null);
            Material newMaterial1 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial1.SetTexture("_BaseMap", newTexture1);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial1);
            texturesManager.ApplyMaterialToObject(cube1, newMaterial1);
            yield return null;
            Texture2D newTexture2 = new Texture2D(2, 2);
            Material newMaterial2 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial2.SetTexture("_BaseMap", newTexture2);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial2);
            yield return null;
            Assert.IsFalse(newTexture1 == null);
        }

        [UnityTest]
        public IEnumerator ShouldDestroyTexture_if_textureIsNotUsedByAnyObject()
        {
            Texture2D newTexture1 = new Texture2D(2, 2);
            Assert.IsFalse(newTexture1 == null);
            Material newMaterial1 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial1.SetTexture("_BaseMap", newTexture1);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial1);
            texturesManager.ApplyMaterialToObject(cube1, newMaterial1);
            yield return null;
            Texture2D newTexture2 = new Texture2D(2, 2);
            Material newMaterial2 = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            newMaterial2.SetTexture("_BaseMap", newTexture2);
            texturesManager.ApplyMaterialToObject(cube0, newMaterial2);
            texturesManager.ApplyMaterialToObject(cube1, newMaterial2);
            yield return null;
            Assert.IsTrue(newTexture1 == null);
        }

        [UnityTest]
        public IEnumerator ShouldNotDestroyTexturesFromProtectedMaterials()
        {
            Texture2D protectedTexture = new Texture2D(2, 2);
            Assert.IsFalse(protectedTexture == null);
            Material protectedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            protectedMaterial.SetTexture("_BaseMap", protectedTexture);
            texturesManager.ApplyProtectedMaterialToObject(cube0, protectedMaterial);
            yield return null;
            Texture2D nonProtectedTexture = new Texture2D(2, 2);
            Material nonProtectedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            nonProtectedMaterial.SetTexture("_BaseMap", nonProtectedTexture);
            texturesManager.ApplyMaterialToObject(cube0, nonProtectedMaterial);
            yield return null;
            Assert.IsFalse(protectedTexture == null);
        }

        [UnityTest]
        public IEnumerator ShouldDestroyNonProtectedTextures_when_applyingProtectedTextures()
        {
            Texture2D nonProtectedTexture = new Texture2D(2, 2);
            Material nonProtectedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            nonProtectedMaterial.SetTexture("_BaseMap", nonProtectedTexture);
            texturesManager.ApplyMaterialToObject(cube0, nonProtectedMaterial);
            yield return null;
            Texture2D protectedTexture = new Texture2D(2, 2);
            Material protectedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            protectedMaterial.SetTexture("_BaseMap", protectedTexture);
            texturesManager.ApplyProtectedMaterialToObject(cube0, protectedMaterial);
            yield return null;
            Assert.IsTrue(nonProtectedTexture == null);
        }

        [UnityTest]
        public IEnumerator ShouldDoNothing_when_RequestedToApplyMaterialToObjectThatAlreadyHasSuchMaterial()
        {
            Texture2D texture = new Texture2D(2, 2);
            Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            Texture appliedTexture;
            material.SetTexture("_BaseMap", texture);

            texturesManager.ApplyMaterialToObject(cube0, material);
            yield return null;
            appliedTexture = cube0.GetComponent<Renderer>().material.GetTexture("_BaseMap");
            Assert.NotNull(appliedTexture);
            Assert.AreEqual(appliedTexture, texture);

            texturesManager.ApplyMaterialToObject(cube0, material);
            yield return null;
            appliedTexture = cube0.GetComponent<Renderer>().material.GetTexture("_BaseMap");

            Assert.NotNull(appliedTexture);
            Assert.AreEqual(appliedTexture, texture);
        }

    }

}