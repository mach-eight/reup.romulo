using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ReupVirtualTwin.enums;

namespace ReupVirtualTwin.helpers
{
    public static class ObjectFinder
    {
        /// <summary>
        /// Find the Object Pool
        /// </summary>
        /// <returns>ObjectPool</returns>
        public static IObjectPool FindObjectPool()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.objectPool).GetComponent<IObjectPool>();
        }

        public static GameObject FindExtensionsTriggers()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.extensionsTriggers);
        }

        public static GameObject FindCharacter()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.character);
        }

        public static GameObject FindSpacesRecord()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.spacesRecord);
        }
        public static GameObject FindObjectRegistry()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.objectRegistry);
        }
        public static GameObject FindEditModeManager()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.editModeManager);
        }
    }
}
