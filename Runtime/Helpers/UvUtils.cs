using System;
using UnityEngine;

namespace ReupVirtualTwin.helpers
{
    public static class UvUtils
    {

        public static void AdjustUVScaleToDimensions(GameObject obj, Vector2 dimensionsInMilimeters)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null)
            {
                return;
            }
            Vector2 dimensionsInMeters = dimensionsInMilimeters / 1000;
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
            UvPointPair[] uvPointPairs = GetSomeUvPointPairs(mesh, 3);
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

            float[,] augmentedMatrix = {
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

            float[,] reducedMatrix = LinearAlgebraUtils.RREF(augmentedMatrix);

            // supposing the transformation we want to return is of the type:
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
        static UvPointPair[] GetSomeUvPointPairs(Mesh mesh, int ammount)
        {
            Vector3[] vertices = mesh.vertices;
            Vector2[] uvs = mesh.uv;
            UvPointPair[] uv3dPairPoints = new UvPointPair[3];
            for (int i = 0; i < ammount; i++)
            {
                uv3dPairPoints[i] = new UvPointPair
                {
                    point = vertices[i],
                    uv = uvs[i]
                };
            }
            return uv3dPairPoints;
        }
        public class UvPointPair
        {
            public Vector2 uv;
            public Vector3 point;
        }

    }
}
