using System.Collections;

namespace ReupVirtualTwin.helpers
{
    public static class YieldUtils
    {
        public static IEnumerator WaitForFrames(int frames)
        {
            for (int i = 0; i < frames; i++)
            {
                yield return null;
            }
        }
    }

}