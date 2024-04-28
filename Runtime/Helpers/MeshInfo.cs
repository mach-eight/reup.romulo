using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.helpers
{
    public class MeshInfo
    {
        public bool isMesh = false;
        public bool isMeshSerialized = false;
        public Vector3? meanVertex = null;
        public ObjectBorder? borders;
        public Vector3? position = null;
        public Vector3 size = Vector3.zero;
        public float volume = 0;
        public float area = 0;
        public float volVertexDensity;
        public float arealVertexDensity;
        public long vertexCount;
        public long triangleCount;


        public MeshInfo(GameObject obj, bool readSerializedMesh = false)
        {
            SerializeMesh serializeMesh = null;
            Mesh mesh;

            if (obj.GetComponent<MeshFilter>() == null)
            {
                isMesh = false;
                return;
            }
            isMesh = true;

            if (readSerializedMesh)
            {
                serializeMesh = obj.GetComponent<SerializeMesh>();
            }
            if (serializeMesh != null)
            {
                mesh = serializeMesh.Rebuild();
                isMeshSerialized = true;
            }
            else
            {
                mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            }
            borders = ReupMeshUtils.GetObjectBorder(mesh, obj.transform);
            if (borders == null) return;
            size = ((ObjectBorder)borders).TransformToCenterSize().size;
            vertexCount = mesh.vertexCount;
            triangleCount = mesh.triangles.Length / 3;
            volVertexDensity = ReupMeshUtils.VolumetricVertexDensity(vertexCount, size);
            arealVertexDensity = ReupMeshUtils.ArealVertexDensity(vertexCount, size);
            area = ReupMeshUtils.Area(size);
            volume = ReupMeshUtils.Volumen(size);
        }
    }
}
