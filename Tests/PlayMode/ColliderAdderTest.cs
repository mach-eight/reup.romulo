using NUnit.Framework;
using ReupVirtualTwin.helpers;
using UnityEngine;

public class ColliderAdderTest
{
    [Test]
    public void AddCollidersToObjectsWithNoColliders()
    {
        //Create objects
        var parentObject = new GameObject();
        MeshFilter parentMeshFilter = parentObject.AddComponent<MeshFilter>();
        parentMeshFilter.sharedMesh = new Mesh();
        for (int i = 0; i < 10; i++)
        {
            var childObject = new GameObject();
            childObject.transform.SetParent(parentObject.transform);
            MeshFilter childMeshFilter = childObject.AddComponent<MeshFilter>();
            childMeshFilter.sharedMesh = new Mesh();
        }

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
