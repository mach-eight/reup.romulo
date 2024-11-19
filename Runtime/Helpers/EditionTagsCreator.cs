using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using System.Collections.Generic;
using System.Linq;

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
                id = 234,
                name = EditionTag.DELETABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateTransformableTag()
        {
            return new Tag()
            {
                id = 233,
                name = EditionTag.TRANSFORMABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateSelectableTag()
        {
            return new Tag()
            {
                id = 232,
                name = EditionTag.SELECTABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
    }
}
