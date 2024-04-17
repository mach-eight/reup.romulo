using ReupVirtualTwin.models;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.enums;

[CustomEditor(typeof(ObjectTags))]
public class ObjectTagsEditor : Editor
{

    private string searchTagText = "";
    private SerializedProperty tagsProperty;
    private Vector2 scrollPosition;

    private const int TAG_BUTTON_HEIGHT = 18;
    private const int MAX_BUTTONS_IN_SCROLL_VIEW = 10;
    private const int UNITY_BUTTON_MARGIN = 2; // This is a variable obtained by trial and error
    private const int BOTTOM_THRESHOLD_IN_PIXELS = 50;

    private ITagsApiManager tagsApiManager;
    private List<string> allTags = new List<string>();

    private async void OnEnable()
    {
        tagsProperty = serializedObject.FindProperty("tags");
        tagsApiManager = ObjectFinder.FindTagsApiManager();
        if (tagsApiManager.webRequester == null)
        {
            tagsApiManager.webRequester = new TagsWebRequesterController(ObjectTags.tagsUrl);
        }
        await GetTags();
    }

    public override async void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
        ShowResetTagsButton();
        ShowCurrentTags();
        EditorGUILayout.Space();
        ShowTagsToAdd();
        if (IsUserAtTheBottomOfScrollView(allTags.Count()))
        {
            await GetMoreTags();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private async void ShowResetTagsButton()
    {
        if (GUILayout.Button("Re fetch tags"))
        {
            allTags = new List<string>();
            tagsApiManager.CleanTags();
            await GetTags();
        }
    }

    private void ShowCurrentTags()
    {
        EditorGUILayout.LabelField("Current tags:");
        for (int i = tagsProperty.arraySize - 1; i >= 0; i--)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tagsProperty.GetArrayElementAtIndex(i).stringValue);
            if (GUILayout.Button("Remove"))
            {
                tagsProperty.DeleteArrayElementAtIndex(i);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    private void ShowTagsToAdd()
    {
        searchTagText = EditorGUILayout.TextField("Search for tag to add:", searchTagText);
        EditorGUILayout.Space();
        int scrollHeight = MAX_BUTTONS_IN_SCROLL_VIEW * (TAG_BUTTON_HEIGHT + UNITY_BUTTON_MARGIN);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));
        var filteredTags = FilterTags();
        foreach (string tag in filteredTags)
        {
            if (GUILayout.Button(tag, GUILayout.Height(TAG_BUTTON_HEIGHT)))
            {
                AddTagIfNotPresent(tag);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private IEnumerable<string> FilterTags()
    {
        return allTags.Where(tag => !IsTagAlreadyPresent(tag) && TagContainsText(tag, searchTagText));
    }

    private async Task GetTags()
    {
        allTags = ApplyEditionTags(await tagsApiManager.GetTags());
    }

    private async Task GetMoreTags()
    {
        allTags = ApplyEditionTags(await tagsApiManager.LoadMoreTags());
    }

    private List<string> ApplyEditionTags(List<ObjectTag> objectTags)
    {
        return new List<string>()
        {
            EditionTag.DELETABLE.ToString(),EditionTag.SELECTABLE.ToString(), EditionTag.TRANSFORMABLE.ToString()
        }
        .Concat(objectTags.Select(tag => tag.name)).ToList();
    }

    private bool TagContainsText(string tag, string text)
    {
        return tag.ToLower().Contains(text.ToLower());
    }

    private void AddTagIfNotPresent(string tag)
    {
        if (!IsTagAlreadyPresent(tag))
        {
            tagsProperty.arraySize++;
            tagsProperty.GetArrayElementAtIndex(tagsProperty.arraySize - 1).stringValue = tag;
        }
    }

    private bool IsTagAlreadyPresent(string tag)
    {
        bool isPresent = false;
        for (int i = 0; i < tagsProperty.arraySize; i++)
        {
            if (tagsProperty.GetArrayElementAtIndex(i).stringValue == tag)
            {
                isPresent = true;
                break;
            }
        }
        return isPresent;
    }

    private bool IsUserAtTheBottomOfScrollView(int numberOfTags)
    {
        int maxScroll = 
            (numberOfTags - MAX_BUTTONS_IN_SCROLL_VIEW) * (TAG_BUTTON_HEIGHT + UNITY_BUTTON_MARGIN) +
            UNITY_BUTTON_MARGIN;
        if (scrollPosition.y >= maxScroll - BOTTOM_THRESHOLD_IN_PIXELS || maxScroll <= UNITY_BUTTON_MARGIN)
        {
            return true;
        }
        return false;
    }
}
