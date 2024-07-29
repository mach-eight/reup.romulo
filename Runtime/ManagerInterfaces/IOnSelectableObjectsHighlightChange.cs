using System;

namespace ReupVirtualTwin.managerInterfaces
{
    public interface IOnSelectableObjectsHighlightChange
    {
        public event Action<bool> SelectableObjectsHighlightChanged;
    }
}
