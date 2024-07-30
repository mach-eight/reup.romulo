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
        public IOnAllowSelectionChange SelectedObjectsManager;

        private void Start()
        {
            SelectedObjectsManager.AllowSelectionChanged += OnSelectionModeChanged;
        }

        private void OnDestroy()
        {
            SelectedObjectsManager.AllowSelectionChanged -= OnSelectionModeChanged;
        }

        private void OnSelectionModeChanged(bool allowSelectionMode)
        {
            selectableObjectHighlighter.enableHighlighting = allowSelectionMode;
        }
    }
}
