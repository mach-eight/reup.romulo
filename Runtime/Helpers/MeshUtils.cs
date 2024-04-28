using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ReupVirtualTwin.helpers;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ReupVirtualTwin.helpers
{
    public static class MeshUtils
    {
        public static Vector3 MeanPoint(GameObject obj)
        {
            var transform = obj.transform;
            var vertices = obj.GetComponent<MeshFilter>().sharedMesh.vertices;
            Vector3 mean = vertices.Aggregate((curr, next) => curr + next) / vertices.Length;
            mean.x = mean.x * transform.GetTotalScale().x;
            mean.y = mean.y * transform.GetTotalScale().y;
            mean.z = mean.z * transform.GetTotalScale().z;
            mean = Quaternion.Euler(transform.localEulerAngles) * mean;
            return mean;
        }

        public static float Volumen(Vector3 size)
        {
            return (size.x * size.y * size.z);
        }

        public static float VolumetricVertexDensity(long vertexNumber, Vector3 size)
        {
            var density = vertexNumber / Volumen(size);
            return density;
        }

        public static float Area(Vector3 size)
        {
            return (size.x * size.y * 2 + size.x * size.z * 2 + size.y * size.z * 2);
        }

        public static float ArealVertexDensity(long vertexNumber, Vector3 size)
        {
            var density = vertexNumber / Area(size);
            return density;
        }

        public static Vector3 Size(Vector3[] borders)
        {
            var size = new Vector3(
                borders[1].x - borders[0].x,
                borders[1].y - borders[0].y,
                borders[1].z - borders[0].z
                );
            return size;
        }

        public static Vector3[] Borders(Mesh mesh, Transform transform)
        {
            var vertices = mesh.vertices;
            if (vertices == null || vertices.Length == 0)
            {
                return null;
            }
            float xmax = vertices[0].x;
            float ymax = vertices[0].y;
            float zmax = vertices[0].z;
            float xmin = vertices[0].x;
            float ymin = vertices[0].y;
            float zmin = vertices[0].z;

            foreach (Vector3 vertex in vertices)
            {
                if (vertex.x < xmin) xmin = vertex.x;
                if (vertex.x > xmax) xmax = vertex.x;
                if (vertex.y < ymin) ymin = vertex.y;
                if (vertex.y > ymax) ymax = vertex.y;
                if (vertex.z < zmin) zmin = vertex.z;
                if (vertex.z > zmax) zmax = vertex.z;
            }

            xmin = xmin * Mathf.Abs(transform.GetTotalScale().x);
            xmax = xmax * Mathf.Abs(transform.GetTotalScale().x);
            ymin = ymin * Mathf.Abs(transform.GetTotalScale().y);
            ymax = ymax * Mathf.Abs(transform.GetTotalScale().y);
            zmin = zmin * Mathf.Abs(transform.GetTotalScale().z);
            zmax = zmax * Mathf.Abs(transform.GetTotalScale().z);

            var minBorder = new Vector3(xmin, ymin, zmin);
            var maxBorder = new Vector3(xmax, ymax, zmax);

            //Since the we are not rotating the wirecube gizmo, rotating this
            //borders is pointless
            //var rotation = Quaternion.Euler(transform.localEulerAngles);
            //minBorder = rotation * minBorder;
            //maxBorder = rotation * maxBorder;

            return new Vector3[] { minBorder, maxBorder };
        }

        public static void GeneratePlanarUVMapping(Mesh mesh)
        {
            var vertexCount = mesh.vertices.Length;
            var vertices = mesh.vertices;
            Vector2[] uvCoords = new Vector2[vertexCount];

            // Generate UV coordinates
            // Here's a simple example of planar mapping
            Vector3 minBounds = new Vector3(float.MaxValue, 0f, float.MaxValue);
            Vector3 maxBounds = new Vector3(float.MinValue, 0f, float.MinValue);

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 vertex = vertices[i];
                minBounds = Vector3.Min(minBounds, vertex);
                maxBounds = Vector3.Max(maxBounds, vertex);
            }

            for (int i = 0; i < vertexCount; i++)
            {
                Vector3 vertex = vertices[i];
                Vector2 uv = new Vector2(
                    Mathf.InverseLerp(minBounds.x, maxBounds.x, vertex.x),
                    Mathf.InverseLerp(minBounds.z, maxBounds.z, vertex.z)
                );
                uvCoords[i] = uv;
            }

            // Assign UV coordinates to the mesh
            mesh.uv = uvCoords;
        }
    }
}