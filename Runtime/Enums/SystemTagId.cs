namespace ReupVirtualTwin.enums
{
    public static class SystemTagId
    {
        public static readonly string SELECTABLE = "[action] selectable";
        public static readonly string TRANSFORMABLE = "[action] transformable";
        public static readonly string DELETABLE = "[action] deletable";
        public static readonly string WALL = "[unity building wall] wall";
        public static string WallFloor(int floorLevel)
        {
            return $"[unity building wall] wall floor {floorLevel}";
        }

        public static string InFloorLocation(int floorLevel)
        {
            return $"[unity building in floor] in floor {floorLevel}";
        }
    }
}
