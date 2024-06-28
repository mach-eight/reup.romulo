using ReupVirtualTwin.models;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.dataModels;

namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(ObjectTags))]
    public class ObjectTagsEditor : Editor
    {

        private ObjectTags objectTags;

        private SelectTagsSection selectTagsSection;

        private async void OnEnable()
        {
            objectTags = (ObjectTags)target;
            ITagsApiManager tagsApiManager = TagsApiManagerEditorFinder.FindTagApiManager();
            selectTagsSection = await SelectTagsSection.Create(tagsApiManager, objectTags.tags);
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
            ShowCurrentTags();
            EditorGUILayout.Space();
            selectTagsSection?.ShowTagsToAdd();
            if (GUI.changed)
            {
                EditorUtility.SetDirty(objectTags);
            }
        }

        private void ShowCurrentTags()
        {
            EditorGUILayout.LabelField("Current tags:");
            List<Tag> tempTags = new List<Tag>(objectTags.tags);
            tempTags.ForEach(tag =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.str);
                if (GUILayout.Button("Remove"))
                {
                    selectTagsSection.RemoveTag(tag);
                }
                EditorGUILayout.EndHorizontal();
            });
        }


    }
}
