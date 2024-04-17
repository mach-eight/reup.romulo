using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using System.Collections.Generic;

namespace ReupVirtualTwin.managers
{
    public class ChangeColorManager : MonoBehaviour, IChangeColorManager
    {
        private IMediator _mediator;
        public IMediator mediator { set => _mediator = value; }

        public void ChangeObjectsColor(List<GameObject> objects, Color color)
        {
            foreach (var obj in objects)
            {
                ChangeObjectColor(obj, color);
            }
            _mediator.Notify(ReupEvent.objectColorChanged);           
        }

        private void ChangeObjectColor(GameObject obj, Color newColor)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                material.color = newColor;
                renderer.material = material;
            }
        }
    }
}