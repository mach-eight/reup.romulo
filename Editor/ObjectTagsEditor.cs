using ReupVirtualTwin.models;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.dataModels;
using System.Linq;

namespace ReupVirtualTwin.editor
{
    [CustomEditor(typeof(ObjectTags))]
    [CanEditMultipleObjects]
    public class ObjectTagsEditor : Editor
    {
        private List<ObjectTags> objectTagsList = new List<ObjectTags>();

        private SelectTagsSection selectTagsSection;

        private async void OnEnable()
        {
            objectTagsList = targets.Cast<ObjectTags>().ToList();
            ITagsApiManager tagsApiManager = TagsApiManagerEditorFinder.FindTagApiManager();
            selectTagsSection = await SelectTagsSection.Create(tagsApiManager, GetCommonTags());
            selectTagsSection.onTagsChange = OnTagsChange;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
            if (!AreAllTagsListsEqual())
            {
                ShowWarning();
            }
            ShowCurrentTags();
            EditorGUILayout.Space();
            selectTagsSection?.ShowTagsToAdd();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnTagsChange(List<Tag> newTags)
        {
            objectTagsList.ForEach(obj =>
            {
                obj.tags = newTags;
                EditorUtility.SetDirty(obj);
            });
        }

        private List<Tag> GetCommonTags()
        {
            HashSet<string> commonTagNames = new HashSet<string>(objectTagsList[0].tags.Select(t => t.name));
            for (int i = 1; i < objectTagsList.Count; i++)
            {
                commonTagNames.IntersectWith(objectTagsList[i].tags.Select(t => t.name));
            }

            return objectTagsList[0].tags
                .Where(t => commonTagNames.Contains(t.name))
                .ToList();
        }

        private bool AreAllTagsListsEqual()
        {
            List<Tag> firstObjectTags = objectTagsList[0].tags;
            foreach (var obj in objectTagsList)
            {
                if (!AreTagsListsEqual(firstObjectTags, obj.tags))
                {
                    return false;
                }
            }
            return true;
        }

        private bool AreTagsListsEqual(List<Tag> firstObjectTags, List<Tag> ObjectTags)
        {
            if (firstObjectTags.Count != ObjectTags.Count)
            {
                return false;
            }
            return firstObjectTags.All(t => ObjectTags.Any(t2 => t2.name == t.name));
        }

        private void ShowCurrentTags()
        {
            EditorGUILayout.LabelField("Current tags:");
            List<Tag> tempTags = GetCommonTags();
            tempTags.ForEach(tag =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.name);
                if (GUILayout.Button("Remove"))
                {
                    selectTagsSection.RemoveTag(tag);
                }
                EditorGUILayout.EndHorizontal();
            });
        }

        private void ShowWarning()
        {
            EditorGUILayout.HelpBox("The selected objects have different tags. Once you start editing the tags, they will be overwritten", MessageType.Warning);
            EditorGUILayout.Space();
            if (GUILayout.Button("Remove tags from all objects"))
            {
                selectTagsSection.RemoveAllTags();
            }
        }


    }
}
