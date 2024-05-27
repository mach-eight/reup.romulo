using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using System.Collections.Generic;

namespace ReupVirtualTwin.helpers
{
    public static class EditionTagsCreator
    {
        public static List<Tag> CreateEditionTags()
        {
            return new List<Tag>()
            {
                CreateDeletableTag(), CreateSelectableTag(), CreateTransformableTag()
            };
        }
        public static Tag CreateDeletableTag()
        {
            return new Tag()
            {
                id = "action-deletable",
                name = EditionTag.DELETABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateTransformableTag()
        {
            return new Tag()
            {
                id = "action-transformable",
                name = EditionTag.TRANSFORMABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateSelectableTag()
        {
            return new Tag()
            {
                id = "action-selectable",
                name = EditionTag.SELECTABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
    }
}
