using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.helpers;
using UnityEngine.TestTools;
using NUnit.Framework;

public class BoundariesUtilsTest
{
    [Test]
    public void CalculateCenter_ReturnsZero_WhenNoRenderers()
    {
        GameObject emptyObj = new GameObject();

        Vector3 center = BoundariesUtils.CalculateCenter(emptyObj);

        Assert.AreEqual(Vector3.zero, center);
    }

    [Test]
    public void CalculateCenter_ReturnsCenter_ForSingleRenderer()
    {
        GameObject obj = new GameObject();
        Renderer renderer = obj.AddComponent<MeshRenderer>();
        renderer.bounds = new Bounds(Vector3.one * 5, Vector3.one * 10);
        
        Vector3 center = BoundariesUtils.CalculateCenter(obj);

        Assert.AreEqual(Vector3.one * 5, center);
    }

    [Test]
    public void CalculateCenter_ReturnsCombinedCenter_ForMultipleRenderers()
    {
        GameObject obj = new GameObject();
        
        GameObject child1 = new GameObject();
        child1.transform.parent = obj.transform;
        Renderer renderer1 = child1.AddComponent<MeshRenderer>();
        renderer1.bounds = new Bounds(Vector3.zero, Vector3.one * 10); // Center (0,0,0), Size (10,10,10)

        GameObject child2 = new GameObject();
        child2.transform.parent = obj.transform;
        Renderer renderer2 = child2.AddComponent<MeshRenderer>();
        renderer2.bounds = new Bounds(Vector3.one * 10, Vector3.one * 10); // Center (10,10,10), Size (10,10,10)

        Vector3 center = BoundariesUtils.CalculateCenter(obj);

        Vector3 expectedCenter = new Vector3(5, 5, 5);
        Assert.AreEqual(expectedCenter, center);
    }
}
