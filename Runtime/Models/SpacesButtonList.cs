using UnityEngine;
using UnityEngine.UI;

using ReupVirtualTwin.models;
using ReupVirtualTwin.helpers;


namespace ReupVirtualTwin.models
{
    public class SpacesButtonList : MonoBehaviour
    {
        public GameObject spaceButtonPrefab;
        public SpacesList spacesList;

        IObjectPool objectPool;
        SpacesRecord spacesRecord;


        private void Start()
        {
            objectPool = ObjectFinder.FindObjectPool();
            spacesRecord = ObjectFinder.FindSpacesRecord().GetComponent<SpacesRecord>();

            spacesRecord.UpdateSpaces();
            foreach (SpaceJumpPoint space in spacesRecord.jumpPoints)
            {
                GameObject spaceButton = objectPool.GetObjectFromPool(spaceButtonPrefab.name, transform);
                var spaceButtonInstance = spaceButton.GetComponent<SpaceButtonInstance>();
                spaceButtonInstance.spaceSelector = space;
                spaceButtonInstance.spaceName = space.spaceName;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

            // Update the size of the parent panel to fit the new content
            RectTransform parentRectTransform = (RectTransform)transform;
            float newHeight = GetComponent<VerticalLayoutGroup>().preferredHeight;
            parentRectTransform.sizeDelta = new Vector2(parentRectTransform.sizeDelta.x, newHeight);
            spacesList.FixHeight(newHeight + 60);
        }
    }
}
