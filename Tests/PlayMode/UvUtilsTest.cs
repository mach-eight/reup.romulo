using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEditor;
using System.Collections;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwinTests.helpers
{
    public class UvUtilsTest
    {
        GameObject wallPrefabInM = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/1_5mx1_5m_inclined_texture_0_1mx0_14m.prefab");
        GameObject wallPrefabInCm = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/150cmx150cm_inclined_texture_10cmx14cm.prefab");
        GameObject wallInstanceInM;
        GameObject wallInstanceInCm;
        Vector2 originalTextureDimensions;

        [SetUp]
        public void SetUp()
        {
            wallInstanceInCm = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefabInCm);
            wallInstanceInM = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefabInM);
            originalTextureDimensions = new Vector2(0.1f, 0.14f);
        }
        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(wallInstanceInCm);
            GameObject.Destroy(wallInstanceInM);
        }

        void AssertVector2sAreEqual(Vector2 expected, Vector2 real)
        {
            float distance = Vector2.Distance(expected, real);
            Assert.IsTrue(distance < 1e-6);
        }

        [Test]
        public void GetTextureDimensions_success_for_fbx_imported_in_cm()
        {
            Vector2 textureDimensions = UvUtils.GetTextureDimensions(wallInstanceInCm);
            AssertVector2sAreEqual(originalTextureDimensions, textureDimensions);
        }

        [Test]
        public void AdjustUVScaleToDimensions_success_for_fbx_imported_in_cm()
        {
            float xScale = 5;
            float yScale = 10;
            Vector2 desiredTextureDimensionsInM = new Vector2(
                originalTextureDimensions.x * xScale, originalTextureDimensions.y * yScale
            );
            Vector2 expectedMaterialScale = new Vector2(1/xScale, 1/yScale);
            UvUtils.AdjustUVScaleToDimensions(wallInstanceInCm, desiredTextureDimensionsInM);
            Material material = wallInstanceInCm.GetComponent<Renderer>().material;
            AssertVector2sAreEqual(expectedMaterialScale, material.mainTextureScale);
        }

        [Test]
        public void GetTextureDimensions_success_for_fbx_imported_in_m()
        {
            Vector2 textureDimensions = UvUtils.GetTextureDimensions(wallInstanceInM);
            AssertVector2sAreEqual(originalTextureDimensions, textureDimensions);
        }

        [Test]
        public void AdjustUVScaleToDimensions_success_for_fbx_imported_in_m()
        {
            float xScale = 5;
            float yScale = 10;
            Vector2 desiredTextureDimensionsInM = new Vector2(
                originalTextureDimensions.x * xScale, originalTextureDimensions.y * yScale
            );
            Vector2 expectedMaterialScale = new Vector2(1/xScale, 1/yScale);
            UvUtils.AdjustUVScaleToDimensions(wallInstanceInM, desiredTextureDimensionsInM);
            Material material = wallInstanceInM.GetComponent<Renderer>().material;
            AssertVector2sAreEqual(expectedMaterialScale, material.mainTextureScale);
        }
    }
}
