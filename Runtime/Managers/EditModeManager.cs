using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using System;
using UnityEngine;

namespace ReupVirtualTwin.managers
{
    public class EditModeManager : MonoBehaviour, IEditModeManager, IOnSelectableObjectsHighlightChange
    {
        public event Action<bool> SelectableObjectsHighlightChanged;
        private bool _editMode = false;
        private bool _subEditMode = false;
        private bool _areSelectableObjectsHighlighted = false;
        public bool editMode {
            get
            {
                return _editMode;
            }
            set
            {
                _editMode = value;
                EmitHighlightSelectableObjectsAction();
                _mediator.Notify(ReupEvent.setEditMode, _editMode);
            }
        }

        public bool subEditMode
        {
            get
            {
                return _subEditMode;
            }
            set
            {
                _subEditMode = value;
                EmitHighlightSelectableObjectsAction();
                _mediator.Notify(ReupEvent.setSubEditMode, _subEditMode);
            }
        }

        private void EmitHighlightSelectableObjectsAction()
        {
            bool shouldHighlightSelectableObjects = _editMode && !_subEditMode;
            if(shouldHighlightSelectableObjects == _areSelectableObjectsHighlighted)
            {
                return;
            }
            _areSelectableObjectsHighlighted = shouldHighlightSelectableObjects;
            SelectableObjectsHighlightChanged?.Invoke(_areSelectableObjectsHighlighted);
        }

        private IMediator _mediator;
        public IMediator mediator { set { _mediator = value; } }

    }
}
