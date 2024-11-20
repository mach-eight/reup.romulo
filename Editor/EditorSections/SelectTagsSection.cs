using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
using ReupVirtualTwin.helpers;
using System;

namespace ReupVirtualTwin.editor
{
    public class SelectTagsSection
    {
        public List<Tag> selectedTags;
        public Action<Tag> onTagDeletion { set => _onTagDeletion = value; }
        public Action<Tag> onTagAddition { set => _onTagAddition = value; }
        public Action onTagReset { set => _onTagReset = value; }


        private ITagsApiManager tagsApiManager;
        private List<Tag> allTags = new List<Tag>();
        private Action<Tag> _onTagDeletion;
        private Action<Tag> _onTagAddition;
        private Action _onTagReset;

        private Vector2 scrollPosition;
        private const int TAG_BUTTON_HEIGHT = 18;
        private const int MAX_BUTTONS_IN_SCROLL_VIEW = 10;
        private const int UNITY_BUTTON_MARGIN = 2; // This is a variable obtained by trial and error
        private const int BOTTOM_THRESHOLD_IN_PIXELS = 50;
        private const int RE_FETCH_BUTTON_WIDTH = 95;

        public string searchTagText = "";

        public async static Task<SelectTagsSection> Create(ITagsApiManager tagsApiManager, List<Tag> referenceToSelectedTags)
        {
            var selectTagsSection = new SelectTagsSection();
            selectTagsSection.tagsApiManager = tagsApiManager;
            selectTagsSection.selectedTags = referenceToSelectedTags;
            await selectTagsSection.GetTags();
            return selectTagsSection;
        }

        public async void ShowTagsToAdd()
        {
            EditorGUILayout.BeginHorizontal();
            searchTagText = EditorGUILayout.TextField("Search for tag to add:", searchTagText);
            ShowRefetchTagsButton();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            int scrollHeight = MAX_BUTTONS_IN_SCROLL_VIEW * (TAG_BUTTON_HEIGHT + UNITY_BUTTON_MARGIN);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));
            IEnumerable<Tag> filteredTags = FilterUnselectedTagsByNameAndId();
            foreach (Tag tag in filteredTags)
            {
                if (GUILayout.Button(tag.str, GUILayout.Height(TAG_BUTTON_HEIGHT)))
                {
                    AddTagIfNotPresent(tag);
                }
            }
            EditorGUILayout.EndScrollView();
            if (IsUserAtTheBottomOfScrollView(filteredTags.Count()) &&
                !tagsApiManager.GetWaitingForTagResponse() &&
                tagsApiManager.GetAreThereTagsToFetch())
            {
                await GetMoreTags();
            }
        }

        public void RemoveTag(Tag tag)
        {
            selectedTags.Remove(tag);
            _onTagDeletion?.Invoke(tag);
        }

        public void RemoveAllTags()
        {
            selectedTags.Clear();
            _onTagReset?.Invoke();
        }

        private async void ShowRefetchTagsButton()
        {
            if (GUILayout.Button("Re fetch tags", GUILayout.Width(RE_FETCH_BUTTON_WIDTH)))
            {
                allTags = new List<Tag>();
                tagsApiManager.CleanTags();
                await tagsApiManager.GetTags();
            }
        }

        private void AddTagIfNotPresent(Tag tag)
        {
            if (!IsTagAlreadyPresent(tag))
            {
                selectedTags.Add(tag);
                _onTagAddition?.Invoke(tag);
            }
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

        private IEnumerable<Tag> FilterUnselectedTagsByNameAndId()
        {
            return allTags.Where(tag =>
            {
                return !IsTagAlreadyPresent(tag) && (
                    StringsUtils.TextContainsSubtext(tag.str, searchTagText)
                    || StringsUtils.MatchRegex(tag.str, searchTagText)
                );
            });
        }

        private async Task GetTags()
        {
            allTags = EditionTagsCreator.ApplyEditionTags(await tagsApiManager.GetTags());
        }

        private async Task GetMoreTags()
        {
            allTags = EditionTagsCreator.ApplyEditionTags(await tagsApiManager.LoadMoreTags());
        }

        private bool IsTagAlreadyPresent(Tag tag)
        {
            return selectedTags.Exists(presentTag => presentTag.id == tag.id);
        }

    }
}
