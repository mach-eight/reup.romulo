using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.webRequestersInterfaces;
using System.Threading.Tasks;
using UnityEngine;
using ReupVirtualTwin.models;
using System;

namespace ReupVirtualTwin.editor
{
    public class TagsApplierUtil
    {
        public static async Task<List<string>> ApplyTags(GameObject building, ITagsApiConsumer tagsApiConsumer)
        {
            List<string> errorMessages = new List<string>();
            try
            {
                Tag[] apiTags = await fetchTags(tagsApiConsumer);
                Dictionary<string, Tag> apiTagsDict = apiTags.ToDictionary(tag => tag.id);
                ApplyTagsToTree(building, apiTagsDict, errorMessages);
            }
            catch (Exception e)
            {
                errorMessages.Add(e.Message);
            }
            return errorMessages;
        }

        private static async Task<Tag[]> fetchTags(ITagsApiConsumer tagsApiConsumer)
        {
            int page = 1;
            int pageSize = 10000;

            PaginationResult<Tag> fetchedTagsResult = await tagsApiConsumer.GetTags(page, pageSize);
            return fetchedTagsResult.results;
        }

        private static void ApplyTagsToTree(GameObject obj, Dictionary<string, Tag> apiTags, List<string> errorMessages)
        {
            ApplyTagsToObject(obj, apiTags, errorMessages);

            foreach (Transform child in obj.transform)
            {
                ApplyTagsToTree(child.gameObject, apiTags, errorMessages);
            }
        }

        private static void ApplyTagsToObject(GameObject obj, Dictionary<string, Tag> apiTags, List<string> errorMessages)
        {
            List<string> objectTagsIds = ExtractTagsIdsFromObject(obj);            
            List<Tag> tagsToApply = objectTagsIds
                .Where(id => apiTags.TryGetValue(id, out _))
                .Select(id => apiTags[id])
                .ToList();

            List<string> notFoundTagsIds = objectTagsIds.Where(id => tagsToApply.All(tag => tag.id != id)).ToList();
            if (notFoundTagsIds.Count > 0)
            {
                string notFoundTagIdsStr = string.Join(" - ", notFoundTagsIds);
                errorMessages.Add($"Tags Ids not found in '{obj.name}': {notFoundTagIdsStr}");
            }

            ObjectTags objectTags = obj.GetComponent<ObjectTags>() ?? obj.gameObject.AddComponent<ObjectTags>();
            Dictionary<string, Tag> existingTagsDict = objectTags.GetTags().ToDictionary(tag => tag.id);
            List<Tag> tagsToAdd = tagsToApply.Where(tag => !existingTagsDict.ContainsKey(tag.id)).ToList();

            objectTags.AddTags(tagsToAdd.ToArray());
        }

        private static List<string> ExtractTagsIdsFromObject(GameObject gameObject)
        {   
            var tagRegex = new Regex(@"TAG_(\d+)", RegexOptions.Compiled);
            return tagRegex.Matches(gameObject.name)
                          .Cast<Match>()
                          .Where(match => match.Success)
                          .Select(match => match.Groups[1].Value)
                          .ToList();
        }
    }
}