using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.behaviourInterfaces;

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
        public static GameObject FindSetupBuilding()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.setupBuilding);
        }
        public static GameObject FindObjectRegistry()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.objectRegistry);
        }
        public static GameObject FindEditModeManager()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.editModeManager);
        }
        public static GameObject FindHeighMediator()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.heightMediator);
        }
        public static ITagsApiManager FindTagsApiManager()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.tagsApiManager).GetComponent<ITagsApiManager>();
        }

        public static IWebMessageReceiver FindWebMessageReceiver()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.editMediator).GetComponent<IWebMessageReceiver>();
        }
        public static GameObject FindDragManager()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.dragManager);
        }
        public static IMediator FindEditMediator()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.editMediator).GetComponent<IMediator>();
        }

        public static GameObject FindGesturesManager()
        {
            return GameObject.FindGameObjectWithTag(TagsEnum.gesturesManager);
        }

    }
}
