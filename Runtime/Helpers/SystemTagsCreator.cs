using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using System.Collections.Generic;
using System.Linq;

namespace ReupVirtualTwin.helpers
{
    public static class SystemTagsCreator
    {
        public static List<Tag> CreateSystemTags()
        {
            return new List<Tag>()
            {
                CreateDeletableTag(), CreateSelectableTag(), CreateTransformableTag(),
                CreateWallTag(),
            };
        }
        public static Tag CreateDeletableTag()
        {
            return new Tag()
            {
                id = SystemTagId.DELETABLE.ToString(),
                name = SystemTagId.DELETABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateTransformableTag()
        {
            return new Tag()
            {
                id = SystemTagId.TRANSFORMABLE.ToString(),
                name = SystemTagId.TRANSFORMABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateSelectableTag()
        {
            return new Tag()
            {
                id = SystemTagId.SELECTABLE.ToString(),
                name = SystemTagId.SELECTABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }

        public static Tag CreateWallTag()
        {
            return new Tag()
            {
                id = SystemTagId.WALL.ToString(),
                name = SystemTagId.WALL.ToString(),
                description = "tag to identify wall faces"
            };
        }

        public static List<Tag> AddSystemTagsToTags(List<Tag> tags)
        {
            return SystemTagsCreator.CreateSystemTags().Concat(tags).ToList();
        }
    }
}
