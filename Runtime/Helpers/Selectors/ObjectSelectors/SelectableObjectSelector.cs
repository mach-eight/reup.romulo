using UnityEngine;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.controllerInterfaces;

namespace ReupVirtualTwin.helpers
{
    public class SelectableObjectSelector : ObjectSelector
    {
        private ITagsController _tagsController;
        public ITagsController tagsController { set =>  _tagsController = value; }
        protected override GameObject GetSelectedObjectFromHitObject(GameObject obj)
        {
            if (_tagsController.DoesObjectHaveTag(obj, EditionTag.SELECTABLE.ToString()))
            {
                return obj;
            }
            if (obj.transform.parent == null)
            {
                return null;
            }
            return GetSelectedObjectFromHitObject(obj.transform.parent.gameObject);
        }

    }
}
