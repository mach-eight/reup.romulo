using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managers;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class ToggleEditMode : MonoBehaviour
    {
        private IEditModeManager _editModeManager;
        void Start()
        {
            _editModeManager = ObjectFinder.FindEditModeManager().GetComponent<IEditModeManager>();
        }

        public void Toggle()
        {
            _editModeManager.editMode = !_editModeManager.editMode;
        }
    }
}
