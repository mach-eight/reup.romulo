using ReupVirtualTwin.models;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequesters;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.dataModels;

[CustomEditor(typeof(ObjectTags))]
public class ObjectTagsEditor : Editor
{

    private ObjectTags objectTags;
    private List<Tag> tagsProperty;
    private Vector2 scrollPosition;

    private const int TAG_BUTTON_HEIGHT = 18;
    private const int MAX_BUTTONS_IN_SCROLL_VIEW = 10;
    private const int UNITY_BUTTON_MARGIN = 2; // This is a variable obtained by trial and error
    private const int BOTTOM_THRESHOLD_IN_PIXELS = 50;

    private ITagsApiManager tagsApiManager;
    private List<Tag> allTags = new List<Tag>();

    private async void OnEnable()
    {
        tagsApiManager = ObjectFinder.FindTagsApiManager();
        if (tagsApiManager.tagsApiConsumer == null)
        {
            tagsApiManager.tagsApiConsumer = new TagsApiConsumer(ObjectTags.tagsUrl);
        }
        await GetTags();
    }

    public override async void OnInspectorGUI()
    {
        objectTags = (ObjectTags)target;
        tagsProperty = objectTags.tags;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"), true, new GUILayoutOption[0]);
        ShowResetTagsButton();
        ShowCurrentTags();
        EditorGUILayout.Space();
        ShowTagsToAdd();
        if (IsUserAtTheBottomOfScrollView(allTags.Count()))
        {
            await GetMoreTags();
        }
        if(GUI.changed)
        {
            EditorUtility.SetDirty(objectTags);
        }
    }

    private async void ShowResetTagsButton()
    {
        if (GUILayout.Button("Re fetch tags"))
        {
            allTags = new List<Tag>();
            tagsApiManager.CleanTags();
            await GetTags();
        }
    }

    private void ShowCurrentTags()
    {
        EditorGUILayout.LabelField("Current tags:");
        List<Tag> tempTags = new List<Tag>(tagsProperty);
        tempTags.ForEach(tag =>
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(tag.name);
            if (GUILayout.Button("Remove"))
            {
                tagsProperty.Remove(tag);
            }
            EditorGUILayout.EndHorizontal();
        });
    }

    private void ShowTagsToAdd()
    {
        tagsApiManager.searchTagText = EditorGUILayout.TextField("Search for tag to add:", tagsApiManager.searchTagText);
        EditorGUILayout.Space();
        int scrollHeight = MAX_BUTTONS_IN_SCROLL_VIEW * (TAG_BUTTON_HEIGHT + UNITY_BUTTON_MARGIN);
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));
        var filteredTags = FilterTags();
        foreach (Tag tag in filteredTags)
        {
            if (GUILayout.Button(tag.name, GUILayout.Height(TAG_BUTTON_HEIGHT)))
            {
                AddTagIfNotPresent(tag);
            }
        }
        EditorGUILayout.EndScrollView();
    }

    private IEnumerable<Tag> FilterTags()
    {
        return allTags.Where(tag => !IsTagAlreadyPresent(tag) && TagContainsText(tag.name, tagsApiManager.searchTagText));
    }

    private async Task GetTags()
    {
        allTags = ApplyEditionTags(await tagsApiManager.GetTags());
    }

    private async Task GetMoreTags()
    {
        allTags = ApplyEditionTags(await tagsApiManager.LoadMoreTags());
    }

    private List<Tag> ApplyEditionTags(List<Tag> tags)
    {
        return EditionTagsCreator.CreateEditionTags().Concat(tags).ToList();
    }

    private bool TagContainsText(string tagName, string text)
    {
        return tagName.ToLower().Contains(text.ToLower());
    }

    private void AddTagIfNotPresent(Tag tag)
    {
        if (!IsTagAlreadyPresent(tag))
        {
            tagsProperty.Add(tag);
        }
    }

    private bool IsTagAlreadyPresent(Tag tag)
    {
        return tagsProperty.Exists(presentTag => presentTag.name == tag.name);
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
