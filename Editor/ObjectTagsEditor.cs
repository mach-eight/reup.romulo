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

        private string CurrentTagsLabel { get => objectTagsList.Count > 1 ? "Common Tags:" : "Current Tags:"; }

        private async void OnEnable()
        {
            objectTagsList = targets.Cast<ObjectTags>().ToList();
            ITagsApiManager tagsApiManager = TagsApiManagerEditorFinder.FindTagApiManager();
            selectTagsSection = await SelectTagsSection.Create(tagsApiManager, GetCommonTags());
            selectTagsSection.onTagDeletion = OnTagDeletion;
            selectTagsSection.onTagAddition = OnTagAddition;
            selectTagsSection.onTagReset = OnTagReset;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
            if (!AreAllTagsListsEqual())
            {
                ShowWarning();
            }
            if (GetDifferentTags().Count > 0)
            {
                ShowDifferentTags();
            }
            ShowCurrentTags();
            EditorGUILayout.Space();
            selectTagsSection?.ShowTagsToAdd();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnTagDeletion(Tag deletedTag)
        {
            objectTagsList.ForEach(objectTags =>
            {
                bool isTagAlreadyPresent = objectTags.tags.Any(t => t.id == deletedTag.id);
                if (isTagAlreadyPresent)
                {
                    objectTags.RemoveTag(deletedTag);
                    EditorUtility.SetDirty(objectTags);
                }
            });
        }

        private void OnTagAddition(Tag addedTag)
        {
            objectTagsList.ForEach(objectTags =>
            {
                bool isTagAlreadyPresent = objectTags.tags.Any(t => t.id == addedTag.id);
                if (!isTagAlreadyPresent)
                {
                    objectTags.AddTag(addedTag);
                    EditorUtility.SetDirty(objectTags);
                }
            });
        }

        private void OnTagReset()
        {
            objectTagsList.ForEach(objectTags =>
            {
                objectTags.tags = new List<Tag>();
                EditorUtility.SetDirty(objectTags);
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

        private List<Tag> GetDifferentTags()
        {
            var commonTagNames = new HashSet<string>(GetCommonTags().Select(t => t.name));
            var differentTags = objectTagsList
                .SelectMany(objectTags => objectTags.tags)
                .Where(t => !commonTagNames.Contains(t.name))
                .Distinct()
                .ToList();

            return differentTags;
        }

        private bool AreAllTagsListsEqual()
        {
            List<Tag> firstObjectTags = objectTagsList[0].tags;
            foreach (var objectTags in objectTagsList)
            {
                if (!AreTagsListsEqual(firstObjectTags, objectTags.tags))
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
            return firstObjectTags.All(t => ObjectTags.Any(t2 => t2.id == t.id));
        }

        private void ShowCurrentTags()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(CurrentTagsLabel);
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
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void ShowDifferentTags()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Tags that are not common to all selected objects:");
            List<Tag> tempTags = GetDifferentTags();
            tempTags.ForEach(tag =>
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(tag.name);
                if (GUILayout.Button("Remove"))
                {
                    OnTagDeletion(tag);
                }
                EditorGUILayout.EndHorizontal();
            });
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }

        private void ShowWarning()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox(
                "The selected objects have different tags.\n " +
                "If you want to create a new set of tags for the selected objects click the " +
                "'Create New Set Of Tags' button", MessageType.Warning);
            if (GUILayout.Button("Create New Set Of Tags"))
            {
                selectTagsSection.RemoveAllTags();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }


    }
}
