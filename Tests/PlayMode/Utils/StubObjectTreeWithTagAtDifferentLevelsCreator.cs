using ReupVirtualTwin.dataModels;
using UnityEngine;

public class StubObjectTreeWithTagAtDifferentLevelsCreator
{
    public static string parentId = "parent-id";
    public static string child0Id = "child0-id";
    public static string child1Id = "child1-id";
    public static string child2Id = "child2-id";
    public static string grandChild00Id = "grandChild00-id";
    public static string grandChild01Id = "grandChild01-id";

    public static Tag tagX = new Tag() { id = "tag-x", name = "tag X" };
    public static Tag tagY = new Tag() { id = "tag-y", name = "tag Y" };
    public static Tag tagZ = new Tag() { id = "tag-z", name = "tag Z" };
    public static Tag notPesentTag = new Tag() { id = "notPesentTag-id", name = "notPesentTag" };

    /// <summary>
    /// Creates a mock object tree with an object with the following structure:
    /// (Objects with X, Y, and Z are objects with those respective tags)
    ///                                        P
    ///                                     /  |  \
    ///                                   C0Z  C1XY  C2
    ///                                  /  \
    ///                               GC00X GC01Y
    /// </summary>
    /// <returns></returns>
    public static GameObject CreateMockObjectWithArbitraryTagAtSecondAndThirdLevel()
    {
        GameObject parent = new(parentId);
        StubObjectCreatorUtils.AssignIdToObject(parent, parentId);

        GameObject child0 = new(child0Id);
        child0.transform.parent = parent.transform;
        StubObjectCreatorUtils.AssignIdToObject(child0, child0Id);
        StubObjectCreatorUtils.AssignTagsToObject(child0, new Tag[] {tagZ});

        GameObject grandChild00 = new(grandChild00Id);
        grandChild00.transform.parent = child0.transform;
        StubObjectCreatorUtils.AssignIdToObject(grandChild00, grandChild00Id);
        StubObjectCreatorUtils.AssignTagsToObject(grandChild00, new Tag[] {tagX});

        GameObject grandChild01 = new(grandChild01Id);
        grandChild01.transform.parent = child0.transform;
        StubObjectCreatorUtils.AssignIdToObject(grandChild01, grandChild01Id);
        StubObjectCreatorUtils.AssignTagsToObject(grandChild01, new Tag[] {tagY});

        GameObject child1 = new(child1Id);
        child1.transform.parent = parent.transform;
        StubObjectCreatorUtils.AssignIdToObject(child1, child1Id);
        StubObjectCreatorUtils.AssignTagsToObject(child1, new Tag[] {tagX, tagY});

        GameObject child2 = new(child2Id);
        child2.transform.parent = parent.transform;
        StubObjectCreatorUtils.AssignIdToObject(child2, child2Id);

        return parent;
    }

}
