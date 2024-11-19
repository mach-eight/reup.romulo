using System.Collections.Generic;
using System.Linq;
using ReupVirtualTwin.dataModels;
using UnityEngine;

namespace ReupVirtualTwinTests.utils
{
    public static class StubObjectTreeCreator
    {
        public static string parentId = "parent-id";
        public static string child0Id = "child0-id";
        public static string child1Id = "child1-id";
        public static string grandChild0Id = "grandChild0-id";

        public static Tag[] parentTags = TagFactory.CreateBulk(3).ToArray();
        public static Tag commonChildrenTag = TagFactory.Create();
        public static Tag[] child0Tags = TagFactory.CreateBulk(3).Concat(new[]{commonChildrenTag}).ToArray();
        public static Tag[] child1Tags = TagFactory.CreateBulk(3).Concat(new[]{commonChildrenTag}).ToArray();
        public static Tag[] grandChild0Tags = TagFactory.CreateBulk(3).ToArray();

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
                deepChild.transform.parent = parent.transform;
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
            StubObjectCreatorUtils.AssignTagsToObject(obj, new Tag[1] { new Tag() { id = objectIndex + 1000, name = $"object-{objectIndex}-tag" } });
            GameObject child = CreateDeepChainedLineOfObjects(depth - 1, objectIndex + 1);
            if (child != null)
            {
                child.transform.parent = obj.transform;
            }
            return obj;
        }
    }
}
