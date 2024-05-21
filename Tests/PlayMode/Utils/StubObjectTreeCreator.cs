using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEngine;

public static class StubObjectTreeCreator
{
    public static string parentId = "parent-id";
    public static string child0Id = "child0-id";
    public static string child1Id = "child1-id";
    public static string grandChild0Id = "grandChild0-id";

    public static Tag[] parentTags = new Tag[3]
    {
        new Tag(){id="parent-tag-0"},
        new Tag(){id="parent-tag-1"},
        new Tag(){id="parent-tag-2"},
    };
    public static Tag commonChildrenTag = new Tag() { id = "common-children-tag" };
    public static Tag[] child0Tags = new Tag[4]
    {
        new Tag(){id="child0-tag-0"},
        new Tag(){id="child0-tag-1"},
        new Tag(){id="child0-tag-2"},
        commonChildrenTag,
    };
    public static Tag[] child1Tags = new Tag[4]
    {
        new Tag(){id="child1-tag-0"},
        new Tag(){id="child1-tag-1"},
        new Tag(){id="child1-tag-2"},
        commonChildrenTag,
    };
    public static Tag[] grandChild0Tags = new Tag[3]
    {
        new Tag(){id="grandChild0-tag-0"},
        new Tag(){id="grandChild0-tag-1"},
        new Tag(){id="grandChild0-tag-2"},
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

        StubObjectCreatorUtils.AssignIdToObject(parent, parentId);
        StubObjectCreatorUtils.AssignIdToObject(child0, child0Id);
        StubObjectCreatorUtils.AssignIdToObject(child1, child1Id);
        StubObjectCreatorUtils.AssignIdToObject(grandChild0, grandChild0Id);

        StubObjectCreatorUtils.AssignTagsToObject(parent, parentTags);
        StubObjectCreatorUtils.AssignTagsToObject(child0, child0Tags);
        StubObjectCreatorUtils.AssignTagsToObject(child1, child1Tags);
        StubObjectCreatorUtils.AssignTagsToObject(grandChild0, grandChild0Tags);

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
        StubObjectCreatorUtils.AssignIdToObject(obj, objectId);
        StubObjectCreatorUtils.AssignTagsToObject(obj, new Tag[1] { new Tag() { id =  $"object-{objectIndex}-tag", name = $"object-{objectIndex}-tag"  } });
        GameObject child = CreateDeepChainedLineOfObjects(depth - 1, objectIndex + 1);
        if (child != null)
        {
            child.transform.parent = obj.transform;
        }
        return obj;
    }
}
