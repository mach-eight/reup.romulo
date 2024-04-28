using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.helpers;

public class MeshUtilsTest : MonoBehaviour
{
    [UnityTest]
    public IEnumerator ExtendBorderWorks()
    {
        ObjectBorder border0 = new ObjectBorder
        {
            minBorders = new Vector3 (-96.47f, 0, -38.55f),
            maxBorders = new Vector3 (-95.97f, 0.42f, -38.45f)
        };
        ObjectBorder border1 = new ObjectBorder
        {
            minBorders = new Vector3 (-96.12f, 0.42f, -38.87f),
            maxBorders = new Vector3 (-95.96f, 0.81f, -38.67f)
        };
        ObjectBorder extendedBorder = (ObjectBorder)ReupMeshUtils.ExtendBorder(border0, border1);
        Assert.AreEqual(new Vector3(-96.47f, 0, -38.87f), extendedBorder.minBorders);
        Assert.AreEqual(new Vector3(-95.96f, 0.81f, -38.45f), extendedBorder.maxBorders);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ExtendBorderReturnsFirstBorderIfSecondBorderIsNull()
    {
        ObjectBorder border0 = new ObjectBorder
        {
            minBorders = new Vector3 (-96.47f, 0, -38.55f),
            maxBorders = new Vector3 (-95.97f, 0.42f, -38.45f)
        };
        ObjectBorder extendedBorder = (ObjectBorder)ReupMeshUtils.ExtendBorder(border0, null);
        Assert.AreEqual(new Vector3(-96.47f, 0, -38.55f), extendedBorder.minBorders);
        Assert.AreEqual(new Vector3(-95.97f, 0.42f, -38.45f), extendedBorder.maxBorders);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ExtendBorderReturnsSecondBorderIfFirstBorderIsNull()
    {
        ObjectBorder border0 = new ObjectBorder
        {
            minBorders = new Vector3 (-96.47f, 0, -38.55f),
            maxBorders = new Vector3 (-95.97f, 0.42f, -38.45f)
        };
        ObjectBorder extendedBorder = (ObjectBorder)ReupMeshUtils.ExtendBorder(null, border0);
        Assert.AreEqual(new Vector3(-96.47f, 0, -38.55f), extendedBorder.minBorders);
        Assert.AreEqual(new Vector3(-95.97f, 0.42f, -38.45f), extendedBorder.maxBorders);
        yield return null;
    }

    [UnityTest]
    public IEnumerator ExtendBorderReturnNullIfBordersAreNull()
    {
        ObjectBorder? extendedBorder = ReupMeshUtils.ExtendBorder(null, null);
        Assert.AreEqual(null, extendedBorder);
        yield return null;
    }
}
