using ReupVirtualTwin.behaviourInterfaces;
using ReupVirtualTwin.modelInterfaces;
using ReupVirtualTwin.models;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class TagSystemAssigner : MonoBehaviour, ITagSystemAssigner
    {
        public IObjectTags AssignTagSystemToObject(GameObject obj)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags == null)
            {
                objectTags = obj.AddComponent<ObjectTags>();
            }
            return objectTags;
        }

        public void AssignTagSystemToTree(GameObject tree)
        {
            AssignTagSystemToObject(tree);
            foreach (Transform child in tree.transform)
            {
                AssignTagSystemToTree(child.gameObject);
            }
        }

        public void RemoveTagSystemFromObject(GameObject obj)
        {
            IObjectTags objectTags = obj.GetComponent<IObjectTags>();
            if (objectTags != null)
            {
                Object.DestroyImmediate((Object)objectTags);
            }
        }

        public void RemoveTagSystemFromTree(GameObject tree)
        {
            RemoveTagSystemFromObject(tree);
            foreach (Transform child in tree.transform)
            {
                RemoveTagSystemFromTree(child.gameObject);
            }
        }
    }
}
