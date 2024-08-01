using System;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IOnAllowSelectionChange
    {
        public event Action<bool> AllowSelectionChanged;
    }
}
