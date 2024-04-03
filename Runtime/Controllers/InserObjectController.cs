using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.dataModels;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.webRequestersInterfaces;

namespace ReupVirtualTwin.controllers
{
    public class InserObjectController : IInsertObjectsController
    {
        IMeshDownloader meshDownloader;
        IMediator mediator;
        public InserObjectController(IMediator mediator, IMeshDownloader meshDownloader)
        {
            this.mediator = mediator;
            this.meshDownloader = meshDownloader;
        }
        public void InsertObject(InsertObjectMessagePayload message)
        {
            InsertObjectRequest request = new InsertObjectRequest(mediator, meshDownloader, message);
        }
    }
}
