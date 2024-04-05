using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEngine;

public static class StubObjectTreeCreator
{
    public static string parentId = "parent-id"; 
    public static string child0Id = "child0-id";
    public static string child1Id = "child1-id";
    public static string grandChild0Id = "grandChild0-id";
    public static string[] parentTags = new string[3]
    {
        "parent-tag-0", "parent-tag-1", "parent-tag-2"
    };
    public static string[] child0Tags = new string[3]
    {
        "child0-tag-0", "child0-tag-1", "child0-tag-2"
    };
    public static string[] child1Tags = new string[3]
    {
        "child1-tag-0", "child1-tag-1", "child1-tag-2"
    };
    public static string[] grandChild0Tags = new string[3]
    {
        "grandChild0-tag-0", "grandChild0-tag-1", "grandChild0-tag-2"
    };

    /// <summary>
    /// Creates a mock building with a parent, two children, a grandchild.
    /// Optionally creates a third child with a deep chained line of objects.
    /// <paramref name="deepChildDepth"/> controls the depth of the third child.
    /// </summary>
    public static GameObject CreateMockBuilding(int deepChildDepth = 0)
    {
        GameObject parent = new(parentId);
        GameObject child0 = new(child0Id);
        child0.transform.parent = parent.transform;
        GameObject child1 = new(child1Id);
        child1.transform.parent = parent.transform;
        GameObject grandChild0 = new(grandChild0Id);
        grandChild0.transform.parent = child0.transform;

        AssignIdToObject(parent, parentId);
        AssignIdToObject(child0, child0Id);
        AssignIdToObject(child1, child1Id);
        AssignIdToObject(grandChild0, grandChild0Id);

        AssignTagsToObject(parent, parentTags);
        AssignTagsToObject(child0, child0Tags);
        AssignTagsToObject(child1, child1Tags);
        AssignTagsToObject(grandChild0, grandChild0Tags);

        if (deepChildDepth > 0)
        {
            GameObject deepChild = CreateDeepChainedLineOfObjects(deepChildDepth);
            deepChild.transform.parent = child1.transform;
        }

        return parent;
    }

    private static GameObject CreateDeepChainedLineOfObjects(int depth, int objectIndex = 0)
    {
        if (depth == 0)
        {
            return null;
        }
        string objectId = $"object-{objectIndex}";
        GameObject obj = new(objectId);
        AssignIdToObject(obj, objectId);
        AssignTagsToObject(obj, new string[1] { $"object-{objectIndex}-tag" });
        GameObject child = CreateDeepChainedLineOfObjects(depth - 1, objectIndex + 1);
        if (child != null)
        {
            child.transform.parent = obj.transform;
        }
        return obj;
    }

    private static void AssignTagsToObject(GameObject obj, string[] tags)
    {
        ITagSystemController tagSystemController = new TagSystemController();
        tagSystemController.AssignTagSystemToObject(obj);
        IObjectTags objectTags = obj.GetComponent<IObjectTags>();
        objectTags.AddTags(tags);
    }

    private static void AssignIdToObject(GameObject obj, string id)
    {
        IUniqueIdentifier identifier = obj.AddComponent<UniqueId>();
        identifier.AssignId(id);
    }
}
