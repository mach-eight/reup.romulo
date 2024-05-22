namespace ReupVirtualTwin.romuloEnvironment
{
    public static class RomuloEnvironment
    {
#if UNITY_EDITOR
        public static readonly bool development = true;
#else
        public static readonly bool development = false;
#endif
    }
}
