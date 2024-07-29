using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.behaviours
{
    public class SelectableObjectsHighlighterEnabler : MonoBehaviour
    {
        [HideInInspector]
        public SensedObjectHighlighter selectableObjectHighlighter;
        public IOnSelectableObjectsHighlightChange editModeManager;

        private void Start()
        {
            editModeManager.SelectableObjectsHighlightChanged += OnSelectableObjectsHighlightChange;
        }

        private void OnDestroy()
        {
            editModeManager.SelectableObjectsHighlightChanged -= OnSelectableObjectsHighlightChange;
        }

        private void OnSelectableObjectsHighlightChange(bool ShouldShowSelectableObjectsChanged)
        {
            selectableObjectHighlighter.enableHighlighting = ShouldShowSelectableObjectsChanged;
        }
    }
}
