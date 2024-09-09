using System;
using UnityEngine;
using AM = Accord.Math;
using System.Collections.Generic;
using System.Linq;

namespace ReupVirtualTwin.helpers
{
    public static class UvUtils
    {

        public static void AdjustUVScaleToDimensions(GameObject obj, Vector2 dimensionsInMillimeters)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }
            Vector2 dimensionsInMeters = dimensionsInMillimeters / 1000;
            Vector2 currentTextureDimensionsInMeters = GetTextureDimensions(obj);
            Vector2 scaleFactor = new Vector2(
                currentTextureDimensionsInMeters.x / dimensionsInMeters.x,
                currentTextureDimensionsInMeters.y / dimensionsInMeters.y
            );
            Material mat = renderer.material;
            mat.mainTextureScale = new Vector2(scaleFactor.x, scaleFactor.y);
        }

        public static Vector2 GetTextureDimensions(GameObject obj)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            UvPointPair[] uvPointPairs = getTriangleWithRespectiveUvs(mesh);
            Func<Vector2, Vector3> uvToSpaceTransformator = CreateUvToSpaceTransformator(uvPointPairs);
            Vector3 uv00ToSpace = uvToSpaceTransformator(new Vector2(0, 0));
            Vector3 uv10ToSpace = uvToSpaceTransformator(new Vector2(1, 0));
            Vector3 uv01ToSpace = uvToSpaceTransformator(new Vector2(0, 1));
            float textureDimensionX = Vector3.Distance(uv10ToSpace, uv00ToSpace);
            float textureDimensionY = Vector3.Distance(uv01ToSpace, uv00ToSpace);
            return new Vector2(textureDimensionX, textureDimensionY);
        }


        static Func<Vector2, Vector3> CreateUvToSpaceTransformator(UvPointPair[] uvPointPairs)
        {
            Vector2 uv0 = uvPointPairs[0].uv;
            Vector2 uv1 = uvPointPairs[1].uv;
            Vector2 uv2 = uvPointPairs[2].uv;
            Vector3 point0 = uvPointPairs[0].point;
            Vector3 point1 = uvPointPairs[1].point;
            Vector3 point2 = uvPointPairs[2].point;

            double[,] augmentedMatrix = {
                {uv0.x, uv0.y, 0, 0, 0, 0, 1, 0, 0, point0.x},
                {0, 0, uv0.x, uv0.y, 0, 0, 0, 1, 0, point0.y},
                {0, 0, 0, 0, uv0.x, uv0.y, 0, 0, 1, point0.z},

                {uv1.x, uv1.y, 0, 0, 0, 0, 1, 0, 0, point1.x},
                {0, 0, uv1.x, uv1.y, 0, 0, 0, 1, 0, point1.y},
                {0, 0, 0, 0, uv1.x, uv1.y, 0, 0, 1, point1.z},

                {uv2.x, uv2.y, 0, 0, 0, 0, 1, 0, 0, point2.x},
                {0, 0, uv2.x, uv2.y, 0, 0, 0, 1, 0, point2.y},
                {0, 0, 0, 0, uv2.x, uv2.y, 0, 0, 1, point2.z},
            };

            double[,] reducedMatrix = new AM.ReducedRowEchelonForm(augmentedMatrix).Result;

            // supposing the transformation we want to return is of the type:
            // ( a b )   ( u )   ( x0 )     ( x )
            // ( c d ) * ( v ) + ( y0 )  =  ( y ) 
            // ( e f )           ( z0 )     ( z )
            double a = reducedMatrix[0, 9];
            double b = reducedMatrix[1, 9];
            double c = reducedMatrix[2, 9];
            double d = reducedMatrix[3, 9];
            double e = reducedMatrix[4, 9];
            double f = reducedMatrix[5, 9];
            double x0 = reducedMatrix[6, 9];
            double y0 = reducedMatrix[7, 9];
            double z0 = reducedMatrix[8, 9];

            return (Vector2 uv) =>
            {
                double u = uv.x;
                double v = uv.y;
                double x = a * u + b * v + x0;
                double y = c * u + d * v + y0;
                double z = e * u + f * v + z0;
                return new Vector3((float)x ,(float)y, (float)z);
            };
        }
        static UvPointPair[] getTriangleWithRespectiveUvs(Mesh mesh)
        {
            int[] triangles = mesh.triangles;
            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            int[] choosenTriangle = FirstNonColinearTriangle(triangles, vertices);
            return choosenTriangle.Select(triangle => new UvPointPair
            {
                uv = uvs[triangle],
                point = vertices[triangle],
            }).ToArray();
        }
        public class UvPointPair
        {
            public Vector2 uv;
            public Vector3 point;
        }

        private static int[] FirstNonColinearTriangle(int[] triangles, Vector3[] vertices)
        {
            int triangleIndex = 0;
            while (triangleIndex < triangles.Length)
            {
                if (!ArePointsCollinear(
                    vertices[triangles[triangleIndex]],
                    vertices[triangles[triangleIndex] + 1],
                    vertices[triangles[triangleIndex] + 2]
                )){
                    return triangles.AsSpan(triangleIndex, 3).ToArray();
                }
                triangleIndex = triangleIndex + 3;
            }
            throw new Exception("no triangle is non collinear");
        }

        private static bool ArePointsCollinear(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 line0 = a - b;
            Vector3 line1 = a - c;
            Vector3 crossProduct = Vector3.Cross(line0, line1);
            if (crossProduct.magnitude < Mathf.Epsilon)
            {
                return true;
            }
            return false;
        }
    }
}
