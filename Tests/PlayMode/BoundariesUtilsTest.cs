using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BoundariesUtilsTest
{
    GameObject parent;
    GameObject child1;
    GameObject child2;

    [SetUp]
    public void Setup()
    {
        parent = new GameObject();
        child1 = new GameObject();
        child2 = new GameObject();
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(parent);
        GameObject.DestroyImmediate(child1);
        GameObject.DestroyImmediate(child2);
    }

    [Test]
    public void CalculateCenter_ReturnsZero_WhenNoRenderers()
    {
        Vector3 center = BoundariesUtils.CalculateCenter(parent);

        Assert.AreEqual(Vector3.zero, center);
    }

    [Test]
    public void CalculateCenter_ReturnsCenter_ForSingleRenderer()
    {
        parent.AddComponent<MeshRenderer>().bounds = new Bounds(Vector3.one * 5, Vector3.one * 10);
        
        Vector3 center = BoundariesUtils.CalculateCenter(parent);

        Assert.AreEqual(Vector3.one * 5, center);
    }

    [Test]
    public void CalculateCenter_ReturnsCombinedCenter_ForMultipleRenderers()
    {
        parent.AddComponent<MeshRenderer>();
        child1.transform.parent = parent.transform;
        child1.AddComponent<MeshRenderer>().bounds = new Bounds(Vector3.zero, Vector3.one * 10); // Center (0,0,0), Size (10,10,10)
        child2.transform.parent = parent.transform;
        child2.AddComponent<MeshRenderer>().bounds = new Bounds(Vector3.one * 10, Vector3.one * 10); // Center (10,10,10), Size (10,10,10)

        Vector3 center = BoundariesUtils.CalculateCenter(parent);

        Vector3 expectedCenter = new Vector3(5, 5, 5);
        Assert.AreEqual(expectedCenter, center);
    }
}
