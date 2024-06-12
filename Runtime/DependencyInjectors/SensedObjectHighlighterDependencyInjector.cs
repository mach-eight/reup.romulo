using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.managers;
using ReupVirtualTwin.romuloEnvironment;
using UnityEngine;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class SensedObjectHighlighterDependencyInjector : MonoBehaviour
    {
        public EditModeManager editModeManager;
        public SelectedObjectsManager selectedObjectsManager;
        private void Awake()
        {
            ObjectSensor objectSensor = new ObjectSensor();
            objectSensor.rayProvider = GetComponent<IRayProvider>();
            SelectableObjectSelector selector = GetComponent<SelectableObjectSelector>();
            selector.tagsController = new TagsController();
            objectSensor.objectSelector = selector;

            SensedObjectHighlighter selectableObjectHighlighter = GetComponent<SensedObjectHighlighter>();
            selectableObjectHighlighter.objectSensor = objectSensor;
            selectableObjectHighlighter.objectHighlighter = new Outliner(RomuloEnvironment.orangeHighlightColor, 5.0f);

            SelectableObjectsHighlighterEnabler selectableObjectHighlighterEnabler = GetComponent<SelectableObjectsHighlighterEnabler>();
            selectableObjectHighlighterEnabler.editModeManager = editModeManager;
            selectableObjectHighlighterEnabler.selectableObjectHighlighter = selectableObjectHighlighter;
            selectableObjectHighlighter.selectedObjectsManager = selectedObjectsManager;
        }
    }
}
