using System;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IGesturesManager
    {
        public bool gestureInProgress { get; }
        public event Action GestureStarted;
        public event Action GestureEnded;
    }
}
