using NUnit.Framework;
using ReupVirtualTwin.helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ReupVirtualTwinTests.helpers
{
    public class LinearAlgebraUtilsTest : MonoBehaviour
    {

        [Test]
        public void ReducedRowEchelonForm_success()
        {
            float[,] matrix = {
                {1, 2, 3, 20 },
                {0, 0, 2, 8},
                {1, 1, 1, 9 }
            };
            float[,] expectedReducedMatrix = {
                {1, 0, 0, 2 },
                {0, 1, 0, 3 },
                {0, 0, 1, 4 },
            };
            float[,] rrefMatrix = LinearAlgebraUtils.RREF(matrix);
            Assert.AreEqual(expectedReducedMatrix, rrefMatrix);
        }
    }
}
