using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;
using ReupVirtualTwin.helpers;

public class VectorUtilsTest : MonoBehaviour
{
    [UnityTest]
    public IEnumerator Vector3MeanWorks()
    {
        List<Vector3> vectors = new List<Vector3>();
        vectors.Add(new Vector3(1,2,3));
        vectors.Add(new Vector3(2,4,6));
        Vector3 mean = VectorUtils.meanVector3(vectors);
        Assert.AreEqual(new Vector3(1.5f, 3, 4.5f), mean);
        yield return null;
    }

    [UnityTest]
    public IEnumerator Vector3MeanWorksFails()
    {
        List<Vector3> vectors = new List<Vector3>();
        vectors.Add(new Vector3(1,2,3));
        vectors.Add(new Vector3(2,4,6));
        Vector3 mean = VectorUtils.meanVector3(vectors);
        Assert.AreEqual(new Vector3(1.5f, 3, 5.0f), mean);
        yield return null;
    }

}
