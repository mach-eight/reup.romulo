using ReupVirtualTwin.managerInterfaces;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.behaviourInterfaces;
using System.Linq;

namespace ReupVirtualTwin.editor
{
    public class TagScannerTool : EditorWindow
    {
        private SelectTagsSection selectTagsSection;
        private List<ITagFilter> tagFilters = new List<ITagFilter>();
        private List<ITagFilter> substringTagFilters = new List<ITagFilter>();
        private IBuildingGetterSetter setupBuilding;
        private SceneVisibilityManager sceneVisibilityManager;
        private List<Tag> selectedTags = new List<Tag>();
        private string subStringFilterText = "";
        private float totalWidth;
        private List<Dictionary<string, bool>> objectsVisibilityStates = new List<Dictionary<string, bool>>();

        [MenuItem("Reup Romulo/Tag Scanner")]
        public static void ShowWindow()
        {
            GetWindow<TagScannerTool>("Tag Scanner");
        }

        private void CreateGUI()
        {
            CreateTagSection();
            SetSetupBuilding();
            sceneVisibilityManager = SceneVisibilityManager.instance;
            SetUpTagFilters(selectedTags);
        }

        private void OnGUI()
        {
            totalWidth = EditorGUIUtility.currentViewWidth;
            ShowApplyButtons();
            EditorGUILayout.Space();
            ShowFilters();
            EditorGUILayout.Space();
            ShowTagsToAdd();
            EditorGUILayout.Space();
            ShowSubStringFilterAdder();
        }
        private void ShowTagsToAdd()
        {
            if (selectTagsSection == null)
            {
                CreateTagSection();
            }
            selectTagsSection?.ShowTagsToAdd();
        }
        private void ShowApplyButtons()
        {
            if (setupBuilding == null)
            {
                SetSetupBuilding();
            }
            GameObject building = setupBuilding.building;
            if (building == null)
            {
                Debug.LogError("No building found");
                return;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Apply filters"))
                {
                    objectsVisibilityStates.Add(ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, new IdController()));
                    ApplyFilters(building);
                }
                if (GUILayout.Button("Restore last objects visibility") && objectsVisibilityStates.Count > 0)
                {
                    UndoLastFilters(building);
                }
            EditorGUILayout.EndHorizontal();
        }

        private void UndoLastFilters(GameObject building)
        {
            Dictionary<string, bool> lastVisibilityState = objectsVisibilityStates.PopLast();
            ObjectVisibilityUtils.ApplyVisibilityState(building, lastVisibilityState, new IdController());
        }

        private void ApplyFilters(GameObject building)
        {
            List<ITagFilter> filters = substringTagFilters.Concat(tagFilters).ToList();
            if (filters.Count == 0)
            {
                Debug.LogWarning("No filters to apply");
                return;
            }
            List<GameObject> filteredObjects = TagFiltersApplier.ApplyFiltersToTree(building, filters);
            sceneVisibilityManager.Hide(building, true);
            for (int i = 0; i < filteredObjects.Count; i++)
            {
                sceneVisibilityManager.Show(filteredObjects[i], true);
            }
        }

        private void ShowFilters()
        {
            float filterNameWidth = totalWidth * 0.6f;
            float removeFilterButtonWidth = totalWidth * 0.2f;
            float invertFilterToggleWidth = totalWidth * 0.2f;
            List<ITagFilter> tempFilters = substringTagFilters.Concat(tagFilters).ToList();
            EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current filters", GUILayout.Width(filterNameWidth));
                EditorGUILayout.LabelField("Remove filter", GUILayout.Width(removeFilterButtonWidth));
                EditorGUILayout.LabelField("Invert filter", GUILayout.Width(invertFilterToggleWidth));
            EditorGUILayout.EndHorizontal();
            tempFilters.ForEach(filter =>
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(filter.displayText, GUILayout.Width(filterNameWidth));
                    if (GUILayout.Button("Remove filter", GUILayout.Width(removeFilterButtonWidth)))
                    {
                        filter.RemoveFilter();
                    }
                    filter.invertFilter = EditorGUILayout.Toggle(filter.invertFilter, GUILayout.Width(invertFilterToggleWidth));
                EditorGUILayout.EndHorizontal();
            });
        }
        private void SetUpTagFilters(List<Tag> tags)
        {
            tagFilters.Clear();
            tags.ForEach(tag =>
            {
                AddTagFilter(tag);
            });
        }
        private void OnTagAddition(Tag tag)
        {
            AddTagFilter(tag);
        }

        private void AddTagFilter(Tag tag)
        {
            ITagFilter tagFilter = new TagFilter(tag);
            tagFilters.Add(tagFilter);
            tagFilter.onRemoveFilter = () =>
            {
                selectedTags.Remove(tag);
                tagFilters.Remove(tagFilter);
            };
        }
        private void SetSetupBuilding()
        {
            setupBuilding = ObjectFinder.FindSetupBuilding().GetComponent<IBuildingGetterSetter>();
        }
        private async void CreateTagSection()
        {
            ITagsApiManager tagsApiManager = TagsApiManagerEditorFinder.FindTagApiManager();
            selectTagsSection = await SelectTagsSection.Create(tagsApiManager, selectedTags);
            selectTagsSection.onTagAddition = OnTagAddition;
        }
        private void ShowSubStringFilterAdder()
        {
            subStringFilterText = EditorGUILayout.TextField("Substring filter", subStringFilterText);
            if (GUILayout.Button("Add substring filter"))
            {
                CreateSubstringTagFilter();
            }
        }

        private void CreateSubstringTagFilter()
        {
            ITagFilter tagFilter = new SubstringTagFilter(subStringFilterText);
            substringTagFilters.Add(tagFilter);
            tagFilter.onRemoveFilter = () =>
            {
                substringTagFilters.Remove(tagFilter);
            };
            subStringFilterText = "";
        }
    }
}
