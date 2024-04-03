using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class InserObjectController : IInsertObjectsController
    {
        IMeshDownloader meshDownloader;
        IMediator mediator;
        Vector3 insertPosition;
        public InserObjectController(IMediator mediator, IMeshDownloader meshDownloader, Vector3 insertPosition)
        {
            this.mediator = mediator;
            this.meshDownloader = meshDownloader;
            this.insertPosition = insertPosition;
        }
        public void InsertObject(InsertObjectMessagePayload message)
        {
            new InsertObjectRequest(mediator, meshDownloader, message, insertPosition);
        }
    }
}
