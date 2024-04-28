using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using UnityEditor;

using ReupVirtualTwin.helpers;
using System.Collections;

public class ObjectWrapperTest : MonoBehaviour
{
    ObjectWrapper objectWrapper;
    GameObject originalParent;
    GameObject cubePrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Packages/com.reup.romulo/Tests/TestAssets/Cube.prefab");
    GameObject obj0;
    GameObject obj1;

    [SetUp]
    public void SetUp()
    {
        objectWrapper = new ObjectWrapper();
        originalParent = new GameObject("original parent");
        obj0 = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        obj0.transform.SetParent(originalParent.transform, true);
        obj1 = (GameObject)PrefabUtility.InstantiatePrefab(cubePrefab);
        obj1.transform.SetParent(originalParent.transform, true);
    }

    [TearDown]
    public void TearDown()
    {
        Destroy(obj0);
        Destroy(obj1);
        Destroy(originalParent);
    }

    [UnityTest]
    public IEnumerator ShouldWrapOneObject()
    {
        objectWrapper.WrapObject(obj0);
        yield return null;
        Assert.IsTrue(objectWrapper.wrappedObjects.Contains(obj0));
        Assert.AreEqual(obj0.transform.parent.name, objectWrapper.wrapper.name);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldWrapTwoObject()
    {
        objectWrapper.WrapObject(obj0);
        objectWrapper.WrapObject(obj1);
        yield return null;
        Assert.IsTrue(objectWrapper.wrappedObjects.Contains(obj0));
        Assert.AreEqual(obj0.transform.parent.name, objectWrapper.wrapper.name);
        Assert.IsTrue(objectWrapper.wrappedObjects.Contains(obj1));
        Assert.AreEqual(obj1.transform.parent.name, objectWrapper.wrapper.name);
        yield return null;
    }
    [UnityTest]
    public IEnumerator ShouldWrapOneObjectAndLeaveOneObject()
    {
        objectWrapper.WrapObject(obj0);
        objectWrapper.WrapObject(obj1);
        yield return null;
        objectWrapper.UnwrapObject(obj1);
        yield return null;
        Assert.IsFalse(objectWrapper.wrappedObjects.Contains(obj1));
        Assert.AreEqual(originalParent.name, obj1.transform.parent.name);
    }
    [UnityTest]
    public IEnumerator ShouldWrapTwoObjectsAndDestroyWrapper()
    {
        objectWrapper.WrapObject(obj0);
        objectWrapper.WrapObject(obj1);
        yield return null;
        objectWrapper.UnwrapObject(obj1);
        objectWrapper.UnwrapObject(obj0);
        yield return null;
        Assert.IsTrue(objectWrapper.wrapper == null);
    }
    [UnityTest]
    public IEnumerator UnwrappedObjectsShouldGoBackToOriginParent()
    {
        objectWrapper.WrapObject(obj0);
        objectWrapper.WrapObject(obj1);
        yield return null;
        objectWrapper.UnwrapObject(obj1);
        objectWrapper.UnwrapObject(obj0);
        yield return null;
        Assert.AreEqual(originalParent.name, obj0.transform.parent.name);
        Assert.AreEqual(originalParent.name, obj1.transform.parent.name);
        yield return null;
    }
}
