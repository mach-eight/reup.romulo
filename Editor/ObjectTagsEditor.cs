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
            ShowCurrentTags();
            EditorGUILayout.Space();
            selectTagsSection?.ShowTagsToAdd();
            serializedObject.ApplyModifiedProperties();
        }

        private void OnTagDeletion(Tag deletedTag)
        {
            objectTagsList.ForEach(objectTags =>
            {
                objectTags.RemoveTag(deletedTag);
                EditorUtility.SetDirty(objectTags);
            });
        }

        private void OnTagAddition(Tag addedTag)
        {
            objectTagsList.ForEach(objectTags =>
            {
                bool isTagAlready = objectTags.tags.Any(t => t.id == addedTag.id);
                if (!isTagAlready)
                {
                    objectTags.AddTag(addedTag);
                    EditorUtility.SetDirty(objectTags);
                }
            });
        }

        private void OnTagReset(List<Tag> resetTags)
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
            EditorGUILayout.LabelField(ShowTextLabel());
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

        private string ShowTextLabel()
        {
            if (objectTagsList.Count > 1) {
                return "Common Tags:";
            }
            return "Current Tags:"; ;
        }

        private void ShowWarning()
        {
            EditorGUILayout.HelpBox(
                "The selected objects have different tags.\n " +
                "If you want to create a new set of tags for the selected objects click the " +
                "'Create New Set Of Tags' button", MessageType.Warning);
            EditorGUILayout.Space();
            if (GUILayout.Button("Create New Set Of Tags"))
            {
                selectTagsSection.RemoveAllTags();
            }
        }


    }
}
