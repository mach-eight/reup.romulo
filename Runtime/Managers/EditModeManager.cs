using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class EditModeManager : MonoBehaviour, IEditModeManager
    {
        private bool _editMode = false;
        public bool editMode {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                _mediator.Notify(ReupEvent.setEditMode, _editMode);
            }
        }

        private IMediator _mediator;
        public IMediator mediator { set { _mediator = value; } }

    }
}
