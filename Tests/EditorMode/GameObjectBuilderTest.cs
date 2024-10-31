using UnityEngine;
using NUnit.Framework;
using System;


namespace ReupVirtualTwinTests.editor
{
    public class GameObjectBuilderTest
    {

        GameObject result;

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(result);
        }

        [Test]
        public void Test_EmptyString_ReturnsSimpleGameObject()
        {
            string input = "";

            result = GameObjectBuilder.CreateGameObjectHierarchy(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(0, result.transform.childCount);
        }

        [Test]
        public void Test_SingleChild_ReturnsGameObjectWithOneChild()
        {
            string input = "()";

            result = GameObjectBuilder.CreateGameObjectHierarchy(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.transform.childCount);
        }

        [Test]
        public void Test_TwoChildren_ReturnsGameObjectWithTwoChildren()
        {
            string input = "()()";

            result = GameObjectBuilder.CreateGameObjectHierarchy(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.transform.childCount);
        }

        [Test]
        public void Test_NestedChild_ReturnsGameObjectWithChildHavingChild()
        {
            string input = "(())";

            result = GameObjectBuilder.CreateGameObjectHierarchy(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.transform.childCount);
            Assert.AreEqual(1, result.transform.GetChild(0).childCount);
        }

        [Test]
        public void Test_InvalidParentheses_ThrowsArgumentException()
        {
            string input = "()(";

            Assert.Throws<ArgumentException>(() => GameObjectBuilder.CreateGameObjectHierarchy(input));
        }

        [Test]
        public void Test_ThreeChildren_ReturnsGameObjectWithThreeChildren()
        {
            string input = "()()()";

            result = GameObjectBuilder.CreateGameObjectHierarchy(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.transform.childCount);
        }

        [Test]
        public void Test_ComplexHierarchy()
        {
            string input = "(()())";

            result = GameObjectBuilder.CreateGameObjectHierarchy(input);

            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.transform.childCount);
            Assert.AreEqual(2, result.transform.GetChild(0).childCount);
        }

        [Test]
        public void NamesShouldBeOrderedDepthFirst()
        {
            string input = "(())(())";
            result = GameObjectBuilder.CreateGameObjectHierarchy(input);
            Assert.AreEqual("0", result.name);
            Assert.AreEqual("1", result.transform.GetChild(0).name);
            Assert.AreEqual("2", result.transform.GetChild(0).GetChild(0).name);
            Assert.AreEqual("3", result.transform.GetChild(1).name);
            Assert.AreEqual("4", result.transform.GetChild(1).GetChild(0).name);
        }
    }

}