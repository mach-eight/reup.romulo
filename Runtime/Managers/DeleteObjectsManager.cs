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
using ReupVirtualTwin.modelInterfaces;

namespace ReupVirtualTwin.managers
{
    public class DeleteObjectsManager : MonoBehaviour, IDeleteObjectsManager

    {
        private IMediator _mediator;
        private IObjectRegistry _registry;
        public IObjectRegistry registry { set => _registry = value; }
        public IMediator mediator { set => _mediator = value; }
        private ITagsController _tagsController;
        public ITagsController tagsController { set => _tagsController = value; }
        

        public List<GameObject> GetDeletableObjects(string stringIDs)
        {
            List<string> listIDs = Utils.ConvertStringToList(stringIDs);
            List<GameObject> gameObjectsToDelete = new();
            if (listIDs != null && listIDs.Count != 0)
            {
                gameObjectsToDelete = _registry.GetObjectsWithGuids(listIDs.ToArray());
                gameObjectsToDelete.RemoveAll(obj => obj == null);
                if (CheckTag(gameObjectsToDelete))
                {
                    return gameObjectsToDelete;
                }
                else
                {
                    gameObjectsToDelete.Clear();
                    return gameObjectsToDelete;
                }
            }
            else
            {
                gameObjectsToDelete.Clear();
                return gameObjectsToDelete;
            }

        }
        
        public bool CheckTag(List<GameObject> objects)
        {
            foreach (var item in objects)
            {
                Debug.Log(item.name);
                var iss = _tagsController.DoesObjectHaveTag(item, EditionTagsCreator.CreateDeletableTag().id);
                Debug.Log(iss);
            }
            return objects.All(obj => _tagsController.DoesObjectHaveTag(obj, EditionTagsCreator.CreateDeletableTag().id));
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