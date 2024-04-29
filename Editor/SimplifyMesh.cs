using UnityEngine;
using UnityEditor;
using UnityMeshSimplifier;
using System;
using TB;
using ReupVirtualTwin.helpers;

namespace ReupVirtualTwin.editor
{
    public class SimplifyMesh : EditorWindow
    {

        GameObject obj;
        float maxVolumetricDensity = 1000000;
        float maxArealDensity = 500000;
        int minVertexCount = 100;
        float softBorderAngle = 60f;
        bool preserveBorderEdges = true;
        bool preserveUVSeamEdges = true;
        bool preserveUVFoldoverEdges = true;
        bool simplifyVerbose = false;

        bool useDefinedQuality;
        float definedQuality = 1f;


        //test variables
        MeshInfo meshInfo;

        [MenuItem("Reup Romulo/Simplify Mesh")]
        public static void ShowWindow()
        {
            GetWindow<SimplifyMesh>("Simplify Mesh");
        }

        void OnGUI()
        {
            if (GUILayout.Button("Simplify"))
            {
                SimplifyAction(false);
            }
            if (GUILayout.Button("Simplify verbose"))
            {
                SimplifyAction(true);
            }
            obj = (GameObject)EditorGUILayout.ObjectField("Object", obj, typeof(object), true);
            preserveBorderEdges = EditorGUILayout.Toggle("Preserve border edges", preserveBorderEdges);
            preserveUVSeamEdges = EditorGUILayout.Toggle("Preserver UV seam edges", preserveUVSeamEdges);
            preserveUVFoldoverEdges = EditorGUILayout.Toggle("Preserver UV foldovers", preserveUVFoldoverEdges);
            softBorderAngle = EditorGUILayout.FloatField("Soft border angle", softBorderAngle);
            useDefinedQuality = EditorGUILayout.Toggle("Use defined quality", useDefinedQuality);
            if (useDefinedQuality)
            {
                definedQuality = EditorGUILayout.FloatField("Quality", definedQuality);
            }
            else
            {
                minVertexCount = EditorGUILayout.IntField("Minimum vertex count", minVertexCount);
                maxArealDensity = EditorGUILayout.FloatField("Max Areal Density", maxArealDensity);
                maxVolumetricDensity = EditorGUILayout.FloatField("Max Volumetric Density", maxVolumetricDensity);
            }
            if (GUILayout.Button("Get Averages"))
            {
                var ave = GetAverageDensity(obj);
                PrintAverages(ave);
            }
        }

        void SimplifyAction(bool verbose)
        {
            simplifyVerbose = verbose;
            if (obj == null)
            {
                Debug.LogError("Please select Object to simplify");
                return;
            }
            var isPrefab = PrefabUtility.IsPartOfPrefabAsset(obj);
            GameObject objToSimplify;
            objToSimplify = obj;
            SimplifyAllTreeObject(objToSimplify);
        }
        void PrintAverages(AverageInfo ave)
        {
            Debug.Log($"number of objects : {ave.numberOfObjects}");
            Debug.Log($"Average Vertex Count : {ave.averageVertexCount}");
            Debug.Log($"Average Triangle Count : {ave.averageTriangleCount}");
            Debug.Log($"Average Vertex Areal Density : {ave.averageVertexArealDensity}");
            Debug.Log($"Average Volumetric Areal Density : {ave.averageVertexVolDenisty}");
        }

        class AverageInfo
        {
            public float numberOfObjects;
            public float averageVertexCount;
            public float averageTriangleCount;
            public float averageVertexVolDenisty;
            public float averageVertexArealDensity;
        }

        AverageInfo GetAverageDensity(GameObject obj)
        {
            float numberOfObjects = 0;
            float averageVertexCount = 0;
            float averageTriangleCount = 0;
            float averageVertexVolDenisty = 0;
            float averageVertexArealDensity = 0;

            var meshInfo = new MeshInfo(obj);
            if (meshInfo.isMesh && meshInfo.arealVertexDensity != float.PositiveInfinity && meshInfo.volVertexDensity != float.PositiveInfinity)
            {
                numberOfObjects = 1;
                averageVertexCount += meshInfo.vertexCount;
                averageTriangleCount += meshInfo.triangleCount;
                averageVertexArealDensity += meshInfo.arealVertexDensity;
                averageVertexVolDenisty += meshInfo.volVertexDensity;
            }

            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                var averageChildInfo = GetAverageDensity(child.gameObject);
                if (averageChildInfo.numberOfObjects == 0) continue;

                var prevNumberofObjects = numberOfObjects;

                //Add to total number of object
                numberOfObjects += averageChildInfo.numberOfObjects;

                //Update avertage vertex Count
                averageVertexCount = averageVertexCount * (prevNumberofObjects / numberOfObjects);
                averageVertexCount += (averageChildInfo.numberOfObjects / numberOfObjects) * averageChildInfo.averageVertexCount;

                //Update avertage vertex Count
                averageTriangleCount = averageTriangleCount * (prevNumberofObjects / numberOfObjects);
                averageTriangleCount += (averageChildInfo.numberOfObjects / numberOfObjects) * averageChildInfo.averageTriangleCount;

                //Update avertage Areal Density
                averageVertexArealDensity = averageVertexArealDensity * (prevNumberofObjects / numberOfObjects);
                averageVertexArealDensity += (averageChildInfo.numberOfObjects / numberOfObjects) * averageChildInfo.averageVertexArealDensity;

                //Update avertage Volumetric Density
                averageVertexVolDenisty = averageVertexVolDenisty * (prevNumberofObjects / numberOfObjects);
                averageVertexVolDenisty += (averageChildInfo.numberOfObjects / numberOfObjects) * averageChildInfo.averageVertexVolDenisty;
                //Debug.Log("vol " + averageVertexVolDenisty);
            }
            return new AverageInfo
            {
                numberOfObjects = numberOfObjects,
                averageVertexCount = averageVertexCount,
                averageTriangleCount = averageTriangleCount,
                averageVertexArealDensity = averageVertexArealDensity,
                averageVertexVolDenisty = averageVertexVolDenisty
            };
        }

        void SimplifyAllTreeObject(GameObject obj)
        {
            SimplifyObjectMesh(obj);
            for (int i = 0; i < obj.transform.childCount; i++)
            {
                Transform child = obj.transform.GetChild(i);
                SimplifyAllTreeObject(child.gameObject);
            }
        }

        void SimplifyObjectMesh(GameObject obj)
        {
            //check that object has meshFilter
            var meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null) return;

            //check if object has either sharedMesh or serializedMesh
            var originalMesh = meshFilter.sharedMesh;

            if (simplifyVerbose)
            {
                Debug.Log($"original mesh for {obj.name}: " + originalMesh.triangles.Length / 3 + " triangles and " + originalMesh.vertices.Length + " vertices");
            }

            float quality = useDefinedQuality ? definedQuality : GetQualityReduction(obj);

            if (quality >= 1)
            {
                if (simplifyVerbose)
                {
                    Debug.Log($"Quality to simplify {obj.name} is {quality}, no reduction was performed");
                }
                return;
            }

            var meshSimplifier = new MeshSimplifier();
            meshSimplifier.Initialize(originalMesh);
            if (simplifyVerbose)
            {
                Debug.Log($"performing reduction in quality {quality} for {obj.name}");
            }
            meshSimplifier.SimplifyMesh(quality);
            meshSimplifier.SimplificationOptions = new SimplificationOptions
            {
                PreserveBorderEdges = preserveBorderEdges,
                PreserveUVSeamEdges = preserveUVFoldoverEdges,
                PreserveUVFoldoverEdges = preserveUVFoldoverEdges,
                PreserveSurfaceCurvature = false,
                EnableSmartLink = true,
                VertexLinkDistance = double.Epsilon,
                MaxIterationCount = 100,
                Agressiveness = 7.0,
                ManualUVComponentCount = false,
                UVComponentCount = 2
            };
            var simplifiedMesh = meshSimplifier.ToMesh();
            //Debug.Log($"simplifiedMesh.vertexCount {simplifiedMesh.vertexCount}");
            //Debug.Log($"simplifiedMesh.vertices.Length {simplifiedMesh.vertices.Length}");
            //Debug.Log($"simplifiedMesh.triangles.Length/3 {simplifiedMesh.triangles.Length/3}");
            //Debug.Log($"simplifiedMesh.normals.Length {simplifiedMesh.normals.Length}");
            //Debug.Log($"simplifiedMesh.uv.Length {simplifiedMesh.uv.Length}");


            var trianglesReduction = 100 * simplifiedMesh.triangles.Length / originalMesh.triangles.Length;
            var vertexReduction = 100 * simplifiedMesh.vertices.Length / originalMesh.vertices.Length;
            if (!(trianglesReduction > 100 || vertexReduction > 100))
            {
                //for some reason, some meshes don't have uvs
                if (simplifiedMesh.uv.Length == 0)
                {
                    ReupVirtualTwin.helpers.ReupMeshUtils.GeneratePlanarUVMapping(simplifiedMesh);
                }
                simplifiedMesh.RecalculateBounds();
                simplifiedMesh.RecalculateNormals(softBorderAngle);
                obj.GetComponent<MeshFilter>().sharedMesh = simplifiedMesh;
            }

            if (simplifyVerbose)
            {
                Debug.Log($"With reduction of {quality} for {obj.name}");
                Debug.Log($"Resulting mesh for {obj.name}: {simplifiedMesh.triangles.Length / 3} triangles ({trianglesReduction}%) \n and {simplifiedMesh.vertices.Length} vertices ({vertexReduction}%)");
                MeshInfo meshInfo = new MeshInfo(obj);
                Debug.Log($"for {obj.name} VolDensity: {meshInfo.volVertexDensity} ArealDensity: {meshInfo.arealVertexDensity}");
            }
        }

        void SimplifyObjectMeshSerialize(GameObject obj)
        {
            //check that object has meshFilter
            var meshFilter = obj.GetComponent<MeshFilter>();
            if (meshFilter == null) return;

            //check if object has either sharedMesh or serializedMesh
            var originalMesh = meshFilter.sharedMesh;
            if (originalMesh == null)
            {
                var sm = obj.GetComponent<SerializeMesh>();
                if (sm != null)
                {
                    originalMesh = sm.Rebuild();
                }
                if (originalMesh == null) return;
            }

            //Make sure that obj has SerializeMesh component
            var serializeMesh = SerializeMeshUtils.GetSerializeMesh(obj);

            //Debug.Log($"original mesh for {obj.name}: " + originalMesh.triangles.Length / 3 + " triangles and " + originalMesh.vertices.Length + " vertices");

            float quality = useDefinedQuality ? definedQuality : GetQualityReduction(obj);

            if (quality >= 1)
            {
                //Debug.Log($"Quality to simplify is {quality}, no reduction was performed");
                return;
            }

            var meshSimplifier = new MeshSimplifier();
            meshSimplifier.Initialize(originalMesh);
            //Debug.Log($"performing reduction in quality {quality} for {obj.name}");
            meshSimplifier.SimplifyMesh(quality);
            meshSimplifier.SimplificationOptions = new SimplificationOptions
            {
                PreserveBorderEdges = preserveBorderEdges,
                PreserveUVSeamEdges = preserveUVFoldoverEdges,
                PreserveUVFoldoverEdges = preserveUVFoldoverEdges,
                PreserveSurfaceCurvature = false,
                EnableSmartLink = true,
                VertexLinkDistance = double.Epsilon,
                MaxIterationCount = 100,
                Agressiveness = 7.0,
                ManualUVComponentCount = false,
                UVComponentCount = 2
            };
            var simplifiedMesh = meshSimplifier.ToMesh();
            //Debug.Log($"simplifiedMesh.vertexCount {simplifiedMesh.vertexCount}");
            //Debug.Log($"simplifiedMesh.vertices.Length {simplifiedMesh.vertices.Length}");
            //Debug.Log($"simplifiedMesh.triangles.Length/3 {simplifiedMesh.triangles.Length/3}");
            //Debug.Log($"simplifiedMesh.normals.Length {simplifiedMesh.normals.Length}");
            //Debug.Log($"simplifiedMesh.uv.Length {simplifiedMesh.uv.Length}");

            //for some reason, some meshes dosen't have uvs
            if (simplifiedMesh.uv.Length == 0)
            {
                ReupVirtualTwin.helpers.ReupMeshUtils.GeneratePlanarUVMapping(simplifiedMesh);
            }
            simplifiedMesh.RecalculateBounds();
            simplifiedMesh.RecalculateNormals(softBorderAngle);
            obj.GetComponent<MeshFilter>().sharedMesh = simplifiedMesh;

            //Serialize the mesh into the object serializeMesh component
            serializeMesh.Serialize();
            var trianglesReduction = 100 * simplifiedMesh.triangles.Length / originalMesh.triangles.Length;
            var vertexReduction = 100 * simplifiedMesh.vertices.Length / originalMesh.vertices.Length;
            if (trianglesReduction > 100 || vertexReduction > 100)
            {
                Debug.Log($"With reduction of {quality}");
                Debug.Log($"Resulting mesh for {obj.name}: {simplifiedMesh.triangles.Length / 3} triangles ({trianglesReduction}%) \n and {simplifiedMesh.vertices.Length} vertices ({vertexReduction}%)");
                MeshInfo meshInfo = new MeshInfo(obj, true);
                Debug.Log($"for {obj.name} VolDensity: {meshInfo.volVertexDensity} ArealDensity: {meshInfo.arealVertexDensity}");
            }
        }

        float GetQualityReduction(GameObject obj)
        {
            MeshInfo meshInfo = new MeshInfo(obj);
            var volDensityQ = maxVolumetricDensity / meshInfo.volVertexDensity;
            var arealDensityQ = maxArealDensity / meshInfo.arealVertexDensity;
            var minDensityQ = Math.Min(volDensityQ, arealDensityQ);

            float vertexCountQ = (float)minVertexCount / (float)meshInfo.vertexCount;
            var quality = Math.Max(minDensityQ, vertexCountQ);

            return Math.Min(quality, 1);
        }

    }
}
