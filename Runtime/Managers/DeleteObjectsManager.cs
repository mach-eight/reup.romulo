using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.helpers;
using ReupVirtualTwin.helperInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.dataModels;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.managers
{
    public class DeleteObjectsManager : MonoBehaviour, IDeleteObjectsManager

    {
        private IMediator _mediator;
        private IRegistry _registry;
        public IRegistry registry { set => _registry = value; }
        public IMediator mediator { set => _mediator = value; }
        private ITagsController _tagsController;
        public ITagsController tagsController { set => _tagsController = value; }

        public List<GameObject>? TryToDeleteObjects(string stringIDs)
        {
            List<string> listIDs = ConvertStringToList(stringIDs);
            if (listIDs != null && listIDs.Count != 0)
            {
                List<GameObject> gameObjectsToDelete = _registry.GetItemsWithGuids(listIDs.ToArray());
                gameObjectsToDelete.RemoveAll(obj => obj == null);
                if (CheckTag(gameObjectsToDelete))
                {
                    return gameObjectsToDelete;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }

        }
        private List<string> ConvertStringToList(string idsString)
        {
            string[] idsArray;
            if (idsString.Length > 0)
            {
                idsArray = idsString.Split(',');
                List<string> idsList = new List<string>(idsArray);
                return idsList;
            }
            else
            {
                return new List<string>();
            }
            
        }
        public bool CheckTag(List<GameObject> objects)
        {
            return objects.All(obj => _tagsController.DoesObjectHaveTag(obj, ObjectTag.DELETABLE));
        }
        public void DeleteObjects(List<GameObject> objectsToDelete)
        {     
            foreach (var obj in objectsToDelete)
            {
                Destroy(obj);
            }
            _mediator.Notify(ReupEvent.objectsDeleted);
        }
    }
}