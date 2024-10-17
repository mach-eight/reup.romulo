using NUnit.Framework;
using UnityEngine;
using UnityEditor;
using ReupVirtualTwin.helpers;
using ReupVirtualTwinTests.utils;

namespace ReupVirtualTwinTests.helpers
{
    public class MaterialScalerTest
    {
        GameObject wallPrefabInM = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/1_5mx1_5m_inclined_texture_0_1mx0_14m.prefab");
        GameObject wallPrefabInCm = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/150cmx150cm_inclined_texture_10cmx14cm.prefab");
        GameObject wallInstanceInM;
        GameObject wallInstanceInCm;
        GameObject testObj;
        MaterialScaler materialScaler;
        Vector2 originalTextureDimensionsInMillimeters;
        Vector2 desiredTextureDimensionsInM;
        Vector2 expectedMaterialScale;

        [SetUp]
        public void SetUp()
        {
            wallInstanceInCm = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefabInCm);
            wallInstanceInM = (GameObject)PrefabUtility.InstantiatePrefab(wallPrefabInM);
            originalTextureDimensionsInMillimeters = new Vector2(100, 140);
            float xScale = 5;
            float yScale = 10;
            desiredTextureDimensionsInM = new Vector2(originalTextureDimensionsInMillimeters.x * xScale, originalTextureDimensionsInMillimeters.y * yScale);
            expectedMaterialScale = new Vector2(1 / xScale, 1 / yScale);
            materialScaler = new MaterialScaler();
            testObj = new GameObject();
        }
        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(wallInstanceInCm);
            GameObject.DestroyImmediate(wallInstanceInM);
            GameObject.DestroyImmediate(testObj);
        }

        [Test]
        public void AdjustUVScaleToDimensions_success_for_fbx_imported_in_cm()
        {
            materialScaler.AdjustUVScaleToDimensions(wallInstanceInCm, desiredTextureDimensionsInM);
            Material material = wallInstanceInCm.GetComponent<Renderer>().material;
            AssertUtils.AssertVectorsAreEqual(expectedMaterialScale, material.mainTextureScale);
        }

        [Test]
        public void AdjustUVScaleToDimensions_success_for_fbx_imported_in_m()
        {
            materialScaler.AdjustUVScaleToDimensions(wallInstanceInM, desiredTextureDimensionsInM);
            Material material = wallInstanceInM.GetComponent<Renderer>().material;
            AssertUtils.AssertVectorsAreEqual(expectedMaterialScale, material.mainTextureScale);
        }

        [Test]
        public void ShouldAdjustUVScaleToDimensions_while_selecting_non_collinear_triangle()
        {
            MeshRenderer meshRenderer = testObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); ;
            MeshFilter meshFilter = testObj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(0,0,0), new Vector3(0,0.5f,0), new Vector3(0,1,0), // 3 points in the same line (same 3 points of first triangle)
                new Vector3(0,1,1),
                new Vector3(0,0,1),
            };
            mesh.uv = new Vector2[] {
                new Vector2(0,0), new Vector2(0.5f,0), new Vector2(1,0), // 3 points in the same line uvs
                new Vector2(1,1),
                new Vector2(0,1),
            };
            mesh.triangles = new int[] {
                0, 1, 2,
                2, 3, 4,
            };
            meshFilter.sharedMesh = mesh;
            materialScaler.AdjustUVScaleToDimensions(testObj, new Vector2(2000, 2000));
            Material material = testObj.GetComponent<Renderer>().material;
            AssertUtils.AssertVectorsAreEqual(new Vector2(0.5f, 0.5f), material.mainTextureScale);
        }

        [Test]
        public void ShouldAdjustUVScaleToDimensions_withTrianglesIndexesNotFollowingAnyOrder()
        {
            MeshRenderer meshRenderer = testObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); ;
            MeshFilter meshFilter = testObj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(0,0,0),
                new Vector3(0,1,1),
                new Vector3(0,0,1),
            };
            mesh.uv = new Vector2[] {
                new Vector2(0,0),
                new Vector2(1,1),
                new Vector2(0,1),
            };
            mesh.triangles = new int[] {
                2,0,1
            };
            meshFilter.sharedMesh = mesh;
            materialScaler.AdjustUVScaleToDimensions(testObj, new Vector2(2000, 2000));
            Material material = testObj.GetComponent<Renderer>().material;
            AssertUtils.AssertVectorsAreEqual(new Vector2(0.5f, 0.5f), material.mainTextureScale);
        }

        [Test]
        public void ShouldAdjustUVScaleToDimensions_even_when_infinitesimalArePresentUVOr3DPoints()
        {
            MeshRenderer meshRenderer = testObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); ;
            MeshFilter meshFilter = testObj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(-14.1f, 2.9f,  0.12f),
                new Vector3(2.40000009340928E-15f, 1, 1),
                new Vector3(1.2999999687846E-10f, 0, 1),
            };
            mesh.uv = new Vector2[] {
                new Vector2(-2.80407665368125E-26f, -2.80407665368125E-26f),
                new Vector2(-2.80407665368125E-26f, 1.06952011585236f),
                new Vector2(0.41801193356514f, 1.06952011585236f),
            };
            mesh.triangles = new int[] {
                0, 1, 2,
            };
            meshFilter.sharedMesh = mesh;
            materialScaler.AdjustUVScaleToDimensions(testObj, new Vector2(2000, 2000));
            Material material = testObj.GetComponent<Renderer>().material;
            AssertUtils.AssertVectorsAreEqual(new Vector2(1.196138f, 6.66403f), material.mainTextureScale);
        }

        [Test]
        public void ShouldMakeSureUVcoordinatesAreNotCollinear()
        {
            MeshRenderer meshRenderer = testObj.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit")); ;
            MeshFilter meshFilter = testObj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mesh.vertices = new Vector3[] {
                new Vector3(0,0,0),
                new Vector3(0,0,1),
                new Vector3(0,1,1),
                new Vector3(0,1,0),
            };
            mesh.uv = new Vector2[] {
                new Vector2(0, 1), // This coordinates are wrong on purpose, it should be 0,0 but this makes the first triangle collinear in uvs
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(1, 0),
            };
            mesh.triangles = new int[] {
                0, 1, 2, // this triangle is collinear in uvs
                1, 2, 3, // this triangle is the one that should be used to calculate the texture dimensions
            };
            meshFilter.sharedMesh = mesh;
            materialScaler.AdjustUVScaleToDimensions(testObj, new Vector2(2000, 2000));
            Material material = testObj.GetComponent<Renderer>().material;
            AssertUtils.AssertVectorsAreEqual(new Vector2(0.5f, 0.5f), material.mainTextureScale);
        }

    }
}
