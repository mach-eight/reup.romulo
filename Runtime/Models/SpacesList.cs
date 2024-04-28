using UnityEngine;

namespace ReupVirtualTwin.models
{
    public class SpacesList : MonoBehaviour
    {
        public void FixHeight(float height)
        {
            var rectTransform = GetComponent<RectTransform>();
            //Since the vertical anchors of the spaceList are attached to the corners of the parent
            //the sizedelta attribute is how much bigger or smaller the rectangle is compared to its parent
            //so that's why this calculation
            var newSizeDeltaY = -(rectTransform.rect.height - height) + rectTransform.sizeDelta.y;

            //if the size is less than cero, it means that new heigh of the SpacesList will be decreased.
            //So decrease the heigh
            if (newSizeDeltaY < 0)
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newSizeDeltaY);

        }

    }
}
