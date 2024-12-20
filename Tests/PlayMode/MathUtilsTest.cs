using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using ReupVirtualTwin.helpers;
using UnityEngine;

namespace ReupVirtualTwinTests.helpers
{
    public class MathUtilsTest
    {
        [Test]
        public void NormalizeAngleTest()
        {
            Assert.AreEqual(0, MathUtils.NormalizeAngle(-360));
            Assert.AreEqual(90, MathUtils.NormalizeAngle(-270));
            Assert.AreEqual(180, MathUtils.NormalizeAngle(180));
            Assert.AreEqual(-90, MathUtils.NormalizeAngle(-90));
            Assert.AreEqual(0, MathUtils.NormalizeAngle(0));
            Assert.AreEqual(90, MathUtils.NormalizeAngle(90));
            Assert.AreEqual(180, MathUtils.NormalizeAngle(180));
            Assert.AreEqual(-90, MathUtils.NormalizeAngle(270));
            Assert.AreEqual(0, MathUtils.NormalizeAngle(360));
        }

        [Test]
        public void NormalizeAngleRadTest()
        {
            float pi = Mathf.PI;
            float errorThreshold = 1e-5f;
            Assert.AreEqual(0, MathUtils.NormalizeAngleRad(-2 * pi), errorThreshold);
            Assert.AreEqual(pi / 2, MathUtils.NormalizeAngleRad(-3 * pi / 2), errorThreshold);
            Assert.AreEqual(pi, MathUtils.NormalizeAngleRad(-pi), errorThreshold);
            Assert.AreEqual(-pi / 2, MathUtils.NormalizeAngleRad(-pi / 2), errorThreshold);
            Assert.AreEqual(0, MathUtils.NormalizeAngleRad(0), errorThreshold);
            Assert.AreEqual(pi / 2, MathUtils.NormalizeAngleRad(pi / 2), errorThreshold);
            Assert.AreEqual(pi, MathUtils.NormalizeAngleRad(pi), errorThreshold);
            Assert.AreEqual(-pi / 2, MathUtils.NormalizeAngleRad(3 * pi / 2), errorThreshold);
            Assert.AreEqual(0, MathUtils.NormalizeAngleRad(2 * pi), errorThreshold);
        }
    }

}