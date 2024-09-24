using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;
using System;
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
            }
            .Concat(CreateFloorWallTags())
            .Concat(CreateFloorTags())
            .ToList();
        }
        public static Tag CreateDeletableTag()
        {
            return new Tag()
            {
                id = "DELETABLE",
                name = SystemTagId.DELETABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateTransformableTag()
        {
            return new Tag()
            {
                id = "TRANSFORMABLE",
                name = SystemTagId.TRANSFORMABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }
        public static Tag CreateSelectableTag()
        {
            return new Tag()
            {
                id = "SELECTABLE",
                name = SystemTagId.SELECTABLE.ToString(),
                description = "this is an action tag burn into unity"
            };
        }

        public static Tag CreateWallTag()
        {
            return new Tag()
            {
                id = "WALL",
                name = SystemTagId.WALL.ToString(),
                description = "tag to identify wall faces"
            };
        }

        public static List<Tag> AddSystemTagsToTags(List<Tag> tags)
        {
            return SystemTagsCreator.CreateSystemTags().Concat(tags).ToList();
        }

        public static Tag CreateFloorLocationTag(int floorLevel)
        {
            return new Tag()
            {
                id = $"FLOOR_{floorLevel}",
                name = SystemTagId.InFloorLocation(floorLevel).ToString(),
                description = $"tag to identify objects in floor {OrdinalNumberUtils.GetOrdinal(floorLevel)}"
            };
        }

        public static List<Tag> CreateFloorTags()
        {
            return Enumerable.Range(1, 11).Select(floor => CreateFloorLocationTag(floor)).ToList();
        }

        public static Tag CreateWallFloorTag(int floorLevel)
        {
            return new Tag()
            {
                id = $"WALL_FLOOR_{floorLevel}",
                name = SystemTagId.WallFloor(floorLevel).ToString(),
                description = $"tag to identify wall in floor {floorLevel}"
            };
        }

        public static List<Tag> CreateFloorWallTags()
        {
            return Enumerable.Range(1, 11).Select(floor => CreateWallFloorTag(floor)).ToList();
        }
    }
}
