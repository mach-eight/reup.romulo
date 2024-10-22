using NUnit.Framework;
using ReupVirtualTwin.helpers;
using UnityEngine;

public class ColliderAdderTest
{
    GameObject parentObject;

    [SetUp]
    public void SetUp()
    {
        parentObject = new GameObject();
        MeshFilter parentMeshFilter = parentObject.AddComponent<MeshFilter>();
        parentMeshFilter.sharedMesh = new Mesh();
        for (int i = 0; i < 10; i++)
        {
            var childObject = new GameObject();
            childObject.transform.SetParent(parentObject.transform);
            MeshFilter childMeshFilter = childObject.AddComponent<MeshFilter>();
            childMeshFilter.sharedMesh = new Mesh();
        }
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.DestroyImmediate(parentObject);
    }

    [Test]
    public void AddCollidersToObjectsWithNoColliders()
    {
        //Check colliders are not there
        Assert.IsNull(parentObject.GetComponent<Collider>());
        foreach (Transform childTransform in parentObject.transform)
        {
            Assert.IsNull(childTransform.gameObject.GetComponent<Collider>());
        }

        ColliderAdder colliderAdder = new ColliderAdder();
        colliderAdder.AddCollidersToTree(parentObject);

        //Check colliders are there
        Assert.IsNotNull(parentObject.GetComponent<Collider>());
        foreach (Transform childTransform in parentObject.transform)
        {
            Assert.IsNotNull(childTransform.gameObject.GetComponent<Collider>());
        }
    }
}
