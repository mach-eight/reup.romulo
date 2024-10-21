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
using System;

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
        private float filterNameWidth;
        private float removeFilterButtonWidth;
        private float toggleFilterPropertyWidth;
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
            DefineDimensions();
            ShowApplyButtons();
            EditorGUILayout.Space();
            ShowFilters();
            EditorGUILayout.Space();
            ShowTagsToAdd();
            EditorGUILayout.Space();
            ShowSubStringFilterAdder();
        }

        private void DefineDimensions()
        {
            totalWidth = EditorGUIUtility.currentViewWidth;
            filterNameWidth = totalWidth * 0.55f;
            removeFilterButtonWidth = totalWidth * 0.15f;
            toggleFilterPropertyWidth = totalWidth * 0.15f;
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
            if (GUILayout.Button("Apply exclusive filters"))
            {
                ApplyFilters(building, TagFiltersApplier.ApplyExclusiveFiltersToTree);
            }
            if (GUILayout.Button("Apply inclusive filters"))
            {
                ApplyFilters(building, TagFiltersApplier.ApplyInclusiveFiltersToTree);
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

        private void ApplyFilters(GameObject building, Func<GameObject, List<ITagFilter>, List<GameObject>> filterFunction)
        {
            objectsVisibilityStates.Add(ObjectVisibilityUtils.GetVisibilityStateOfAllObjects(building, new IdController()));
            List<ITagFilter> filters = substringTagFilters.Concat(tagFilters).ToList();
            if (filters.Count == 0)
            {
                Debug.LogWarning("No filters to apply");
                return;
            }
            List<GameObject> filteredObjects = filterFunction(building, filters);
            sceneVisibilityManager.Hide(building, true);
            for (int i = 0; i < filteredObjects.Count; i++)
            {
                sceneVisibilityManager.Show(filteredObjects[i], true);
            }
        }

        private void ShowFilters()
        {
            List<ITagFilter> tempFilters = substringTagFilters.Concat(tagFilters).ToList();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("activated", GUILayout.Width(toggleFilterPropertyWidth));
            EditorGUILayout.LabelField("Filter look for", GUILayout.Width(filterNameWidth));
            EditorGUILayout.LabelField("Remove filter", GUILayout.Width(removeFilterButtonWidth));
            EditorGUILayout.LabelField("Invert filter", GUILayout.Width(toggleFilterPropertyWidth));
            EditorGUILayout.EndHorizontal();
            tempFilters.ForEach(filter => ShowFilterRow(filter));
        }

        private void ShowFilterRow(ITagFilter filter)
        {
            EditorGUILayout.BeginHorizontal();
            filter.filterIsActive = EditorGUILayout.Toggle(filter.filterIsActive, GUILayout.Width(toggleFilterPropertyWidth));
            GUIStyle dimmedLabelStyle = new GUIStyle(GUI.skin.label);
            if (!filter.filterIsActive)
            {
                dimmedLabelStyle.normal.textColor = Color.gray;
            }
            EditorGUILayout.LabelField(filter.displayText, dimmedLabelStyle, GUILayout.Width(filterNameWidth));
            if (GUILayout.Button("Remove filter", GUILayout.Width(removeFilterButtonWidth)))
            {
                filter.RemoveFilter();
            }
            filter.invertFilter = EditorGUILayout.Toggle(filter.invertFilter, GUILayout.Width(toggleFilterPropertyWidth));
            EditorGUILayout.EndHorizontal();
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
