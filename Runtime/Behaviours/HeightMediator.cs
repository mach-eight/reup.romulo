using ReupVirtualTwin.controllerInterfaces;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using System;
using UnityEngine;

namespace ReupVirtualTwin.behaviours
{
    public class HeightMediator : MonoBehaviour, IMediator, ICharacterHeightReseter
    {
        private ICharacterColliderController _colliderController;
        private IMaintainHeight _maintainHeight;
        private IInitialSpawn _initialSpawn;
        private LayerMask _buildingLayerMask;
        private float minHeight = 0.15f;
        private float _ceilCheckHeight = -0.05f;
        private float _ceilCheckRadius = 0.12f;
        private float _characterHeight;
        public float CharacterHeight { get => _characterHeight; }
        [Range(0.15f, 3f)]
        public float initialCharacterHeight = 1.75f;

        public ICharacterColliderController colliderController { set { _colliderController = value; } }
        public IMaintainHeight maintainHeight { set { _maintainHeight = value; } }
        public IInitialSpawn initialSpawn { set { _initialSpawn = value; } }
        public LayerMask buildingLayerMask { set =>  _buildingLayerMask = value; }

        private void Start()
        {
            ResetCharacterHeight();
            _initialSpawn.Spawn();
        }

        public void Notify(ReupEvent eventName)
        {
            switch (eventName)
            {
                case ReupEvent.setCharacterHeight:
                    updateHeight();
                    break;
                default:
                    break;
            }
        }
        public void Notify<T>(ReupEvent eventName, T payload)
        {
            switch(eventName)
            {
                case ReupEvent.addToCharacterHeight:
                    AddToHeight((float)(object)payload);
                    break;
                default:
                    break;
            }
        }
        private void updateHeight()
        {
            _colliderController.UpdateCollider(_characterHeight);
            _maintainHeight.characterHeight = _characterHeight;
        }
        private void AddToHeight(float heightDelta)
        {
            Boolean minHeightGuard = _characterHeight + heightDelta < minHeight;
            Boolean ceilGuard = Physics.CheckSphere(getCeilCheckPosition(), _ceilCheckRadius, _buildingLayerMask) && heightDelta > 0;
            if (minHeightGuard)
            {
                Debug.LogWarning($"character has reached it's mininum allowed height of {minHeight} m.");
                return;
            }
            if(ceilGuard)
            {
                Debug.LogWarning("character can not increase any further it's height because of ceil collision");
                return;
            }
            _colliderController.DestroyCollider();
            _characterHeight += heightDelta;
            _maintainHeight.characterHeight = _characterHeight;
        }
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(getCeilCheckPosition(), _ceilCheckRadius);
        }

        private Vector3 getCeilCheckPosition()
        {
            return new Vector3(transform.position.x, transform.position.y + _ceilCheckHeight, transform.position.z);
        }

        public void ResetCharacterHeight()
        {
            _characterHeight = initialCharacterHeight;
            updateHeight();
        }
    }
}
