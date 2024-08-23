using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwinTests.helpers
{
    public class MaterialScalerTest
    {
        GameObject wallPrefabInM = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/1_5mx1_5m_inclined_texture_0_1mx0_14m.prefab");
        GameObject wallPrefabInCm = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/150cmx150cm_inclined_texture_10cmx14cm.prefab");
        GameObject wallInstanceInM;
        GameObject wallInstanceInCm;
        Vector2 originalTextureDimensions;
        MaterialScaler materialScaler;
        Vector2 desiredTextureDimensionsInM;
        Vector2 expectedMaterialScale;

        [SetUp]
        public void SetUp()
        {
            wallInstanceInCm = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefabInCm);
            wallInstanceInM = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefabInM);
            originalTextureDimensions = new Vector2(0.1f, 0.14f);
            float xScale = 5;
            float yScale = 10;
            desiredTextureDimensionsInM = new Vector2(originalTextureDimensions.x * xScale, originalTextureDimensions.y * yScale);
            expectedMaterialScale = new Vector2(1/xScale, 1/yScale);
            materialScaler = new MaterialScaler();
        }
        [TearDown]
        public void TearDown()
        {
            GameObject.Destroy(wallInstanceInCm);
            GameObject.Destroy(wallInstanceInM);
        }

        [Test]
        public void AdjustUVScaleToDimensions_success_for_fbx_imported_in_cm()
        {
            materialScaler.AdjustUVScaleToDimensions(wallInstanceInCm, desiredTextureDimensionsInM);
            Material material = wallInstanceInCm.GetComponent<Renderer>().material;
            AssertUtils.AssertVector2sAreEqual(expectedMaterialScale, material.mainTextureScale);
        }

        [Test]
        public void AdjustUVScaleToDimensions_success_for_fbx_imported_in_m()
        {
            materialScaler.AdjustUVScaleToDimensions(wallInstanceInM, desiredTextureDimensionsInM);
            Material material = wallInstanceInM.GetComponent<Renderer>().material;
            AssertUtils.AssertVector2sAreEqual(expectedMaterialScale, material.mainTextureScale);
        }
    }
}
