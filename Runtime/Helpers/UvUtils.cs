using System;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class UvUtils
    {

        public static void AdjustUVScaleToDimensions(GameObject obj, Vector2 dimensionsInCm)
        {
            Vector2 currentTextureDimensionsInCm = GetTextureDimensions(obj);
            Vector2 scaleFactor = new Vector2(
                currentTextureDimensionsInCm.x / dimensionsInCm.x,
                currentTextureDimensionsInCm.y / dimensionsInCm.y
            );
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material mat = renderer.material;
                mat.mainTextureScale = new Vector2(scaleFactor.x, scaleFactor.y);
            }
        }

        static Vector2 GetTextureDimensions(GameObject obj)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;
            UV3DPairPoints[] references = Get3TransormationReferences(mesh);
            Func<Vector2, Vector3> uvToSpaceTransformator = CreateUvToSpaceTransformator(references);
            Vector3 uv00ToSpace = uvToSpaceTransformator(new Vector2(0, 0));
            Vector3 uv10ToSpace = uvToSpaceTransformator(new Vector2(1, 0));
            Vector3 uv01ToSpace = uvToSpaceTransformator(new Vector2(0, 1));
            float textureDimensionX = Vector3.Distance(uv10ToSpace, uv00ToSpace);
            float textureDimensionY = Vector3.Distance(uv01ToSpace, uv00ToSpace);
            return new Vector2(textureDimensionX, textureDimensionY);
        }


        static Func<Vector2, Vector3> CreateUvToSpaceTransformator(UV3DPairPoints[] references)
        {
            Vector2 uv0 = references[0].uv;
            Vector2 uv1 = references[1].uv;
            Vector2 uv2 = references[2].uv;
            Vector3 point0 = references[0].point;
            Vector3 point1 = references[1].point;
            Vector3 point2 = references[2].point;

            float[,] matrix = {
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

            float[,] reducedMatrix = LinearAlgebraUtils.RREF(matrix);

            // supposing that the transformation we want to return is of the type:
            // ( a b )   ( u )   ( x0 )     ( x )
            // ( c d ) * ( v ) + ( y0 )  =  ( y ) 
            // ( e f )           ( z0 )     ( z )
            float a = reducedMatrix[0, 9];
            float b = reducedMatrix[1, 9];
            float c = reducedMatrix[2, 9];
            float d = reducedMatrix[3, 9];
            float e = reducedMatrix[4, 9];
            float f = reducedMatrix[5, 9];
            float x0 = reducedMatrix[6, 9];
            float y0 = reducedMatrix[7, 9];
            float z0 = reducedMatrix[8, 9];


            return (Vector2 uv) =>
            {
                float u = uv.x;
                float v = uv.y;
                float x = a * u + b * v + x0;
                float y = c * u + d * v + y0;
                float z = e * u + f * v + z0;
                return new Vector3(x,y,z);
            };
        }
        static UV3DPairPoints[] Get3TransormationReferencesdummy()
        {
            return new UV3DPairPoints[]
            {
                new UV3DPairPoints
                {
                    uv = new Vector2(10f, 13.0718954248366f),
                    point = new Vector3(0,0,-141.42135623731f),
                },
                new UV3DPairPoints
                {
                    uv = new Vector2(0, 13.0718954248366f),
                    point = new Vector3(-70.7106781186548f, 0, -70.7106781186547f),
                },
                new UV3DPairPoints
                {
                    uv = new Vector2(0,0),
                    point = new Vector3(0,0,0),
                },
            };
        }
        static UV3DPairPoints[] Get3TransormationReferences(Mesh mesh)
        {

            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;

            UV3DPairPoints[] uv3dPairPoints = new UV3DPairPoints[3];

            for (int i = 0; i < 3; i++)
            {
                uv3dPairPoints[i] = new UV3DPairPoints
                {
                    point = vertices[i],
                    uv = uvs[i]
                };
            }
            return uv3dPairPoints;

        }
        private class UV3DPairPoints
        {
            public Vector2 uv;
            public Vector3 point;
        }

        public static Vector2 GetUVBounds(GameObject obj)
        {
            Mesh mesh = obj.GetComponent<MeshFilter>().sharedMesh;

            if (mesh == null)
            {
                Debug.LogError("Object does not have a MeshFilter with a mesh.");
                return Vector2.one;
            }

            Vector2 uvMin = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 uvMax = new Vector2(float.MinValue, float.MinValue);

            foreach (Vector2 uv in mesh.uv)
            {
                uvMin = Vector2.Min(uvMin, uv);
                uvMax = Vector2.Max(uvMax, uv);
            }

            return uvMax - uvMin; // Returns the UV bounds (width and height)
        }

    }
}
