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
    public static GameObject CreateMockBuilding()
    {
        IIdAssignerController idAssigner = new IdController();
        ITagSystemController tagSystemController = new TagSystemController();

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

        tagSystemController.AssignTagSystemToTree(parent);
        parent.GetComponent<IObjectTags>().AddTags(parentTags);
        child0.GetComponent<IObjectTags>().AddTags(child0Tags);
        child1.GetComponent<IObjectTags>().AddTags(child1Tags);
        grandChild0.GetComponent<IObjectTags>().AddTags(grandChild0Tags);

        return parent;
    }

    private static void AssignIdToObject(GameObject obj, string id)
    {
        IUniqueIdentifier identifier = obj.AddComponent<UniqueId>();
        identifier.AssignId(id);
    }
}
