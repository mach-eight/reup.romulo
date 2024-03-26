using UnityEngine;
using ReupVirtualTwin.managerInterfaces;
using ReupVirtualTwin.enums;
using System.Collections.Generic;
using ReupVirtualTwin.models;

namespace ReupVirtualTwin.managers
{
    public class ChangeColorManager : MonoBehaviour, IChangeColorManager
    {
        private IMediator _mediator;
        public IMediator mediator { set => _mediator = value; }
        private IRegistry _registry;
        public IRegistry registry { set => _registry = value; }

        public List<GameObject> GetObjectsToChangeColor(List<string> stringIDs)
        {
            return _registry.GetItemTreesWithParentGuids(stringIDs);
        }

        public void ChangeObjectsColor(List<GameObject> objectsToPaint, Color color)
        {
            foreach (var obj in objectsToPaint)
            {
                ChangeObjectColor(obj, color);
            }
            _mediator.Notify(ReupEvent.objectColorChanged);           
        }

        public Color? parseColor(string colorString)
        {
            if(ColorUtility.TryParseHtmlString(colorString, out Color parsedColor))
            {
                return parsedColor;
            }
            else
            {
                return null;
            }
             
        }
        private void ChangeObjectColor(GameObject obj, Color newColor)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = newColor;
            }
        }
    }
}