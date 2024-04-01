using ReupVirtualTwin.models;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.controllers;
using ReupVirtualTwin.enums;

[CustomEditor(typeof(ObjectTags))]
public class ObjectTagsEditor : Editor
    {
    private string searchTagText = "";
    private SerializedProperty tagsProperty;
    private Vector2 scrollPosition;
    private int TAG_BUTTON_HEIGHT = 18;
    private int MAX_BUTTONS_IN_SCROLL_VIEW = 10;
    private int UNITY_BUTTON_MARGIN = 2; // This is a variable obtained by trial and error
    private ITagsApiManager tagsApiManager;
    private List<string> allTags = new List<string>();
    private bool tagsRquested = false;

    private async void OnEnable()
    {
        tagsProperty = serializedObject.FindProperty("tags");
        tagsApiManager = FinderController.FindTagsApiManager();
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
        if (IsUserAtTheBottomOfScrollView(allTags.Count(), scrollPosition.y) && !tagsRquested)
        {
            Debug.Log("looks like user is at the bottom of the scroll view");
            await GetMoreTags();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private async void ShowResetTagsButton()
    {
        if (GUILayout.Button("Re fetch tags"))
        {
            Debug.Log("reseeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeet");
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

    //private async void ShowTagsToAdd()
    private void ShowTagsToAdd()
    {
        searchTagText = EditorGUILayout.TextField("Search for tag to add:", searchTagText);
        EditorGUILayout.Space();
        int scrollHeight = MAX_BUTTONS_IN_SCROLL_VIEW * (TAG_BUTTON_HEIGHT + UNITY_BUTTON_MARGIN);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));
        //await GetMoreTags();
        //await GetTags();
        var filteredTags = allTags.Where(tag => !IsTagAlredyPresent(tag) && TagContainsText(tag, searchTagText));
        foreach (string tag in filteredTags)
        {
            if (GUILayout.Button(tag, GUILayout.Height(TAG_BUTTON_HEIGHT)))
            {
                AddTagIfNotPresent(tag);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private async Task GetTags()
    {
        tagsRquested = true;
        allTags = (await tagsApiManager.GetTags())
            .Select(tag => tag.name)
            //.Where(tag => TagContainsText(tag, searchTagText) && !IsTagAlredyPresent(tag))
            //.Where(tag => TagContainsText(tag, searchTagText))
            .ToList();
        Debug.Log("after getting first tags");
        Debug.Log("number of all Tags is " + allTags.Count);
        tagsRquested = false;
    }

    private async Task GetMoreTags()
    {
        tagsRquested = true;
        allTags = (await tagsApiManager.LoadMoreTags())
            .Select(tag => tag.name)
            //.Where(tag => TagContainsText(tag, searchTagText) && !IsTagAlredyPresent(tag))
            .ToList();
        Debug.Log("after getting even moreeee tags");
        Debug.Log("number of all Tags is " + allTags.Count);
        tagsRquested = false;
    }

    private bool TagContainsText(string tag, string text)
    {
        return tag.ToLower().Contains(text.ToLower());
    }

    private void AddTagIfNotPresent(string tag)
    {
        if (!IsTagAlredyPresent(tag))
        {
            tagsProperty.arraySize++;
            tagsProperty.GetArrayElementAtIndex(tagsProperty.arraySize - 1).stringValue = tag;
        }
    }

    private bool IsTagAlredyPresent(string tag)
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

    private bool IsUserAtTheBottomOfScrollView(int numberOfTags, float scrollY)
    {
        int maxScroll = 
            (numberOfTags - MAX_BUTTONS_IN_SCROLL_VIEW) * (TAG_BUTTON_HEIGHT + UNITY_BUTTON_MARGIN) +
            UNITY_BUTTON_MARGIN;
        int bottomThresholdInPixels = 100;
        if (scrollY >= maxScroll - bottomThresholdInPixels)
        {
            return true;
        }
        return false;
    }
}
