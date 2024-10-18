using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ReupVirtualTwin.models;
using UnityEditor;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managers;


namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(TagsApiManager))]
    public class TagApiManagerEditor : Editor
    {
        private TagsApiManager tagsApiManager;
        private void OnEnable()
        {
            tagsApiManager = (TagsApiManager)target;
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (GUILayout.Button("Reset Tags Api Manager"))
            {
                tagsApiManager.Reset();
            }
        }
    }
}
