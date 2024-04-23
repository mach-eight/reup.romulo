using ReupVirtualTwin.controllerInterfaces;
using UnityEngine;

namespace ReupVirtualTwin.controllers
{
    public class CharacterColliderBoxController : ICharacterColliderController

    {
        GameObject character;
        float FREE_SPACE_BELOW_IN_METERS = 1f;
        float MIN_COLLIDER_HEIGHT = 0.05f;
        float COLLIDER_FRONT_IN_METERS = 0.02f;
        float COLLIDER_SIDE_IN_METERS = 0.2f;
        float EXTRA_HEIGHT_ABOVE_CHARACTER = -0.05f;

        public CharacterColliderBoxController(GameObject character)
        {
            this.character = character;
        }

        public bool UpdateCollider(float characterHeight)
        {
            if (IsCharacterTooLow(characterHeight))
            {
                return false;
            }
            DestroyCollider();
            return CreateCollider(characterHeight);
        }

        public void DestroyCollider()
        {
            Collider collider = character.GetComponent<Collider>();
            if (collider != null)
            {
                GameObject.Destroy(collider);
            }
        }

        private bool CreateCollider(float characterHeight)
        {
            BoxCollider collider = character.AddComponent<BoxCollider>();
            float colliderHeight = characterHeight - FREE_SPACE_BELOW_IN_METERS + EXTRA_HEIGHT_ABOVE_CHARACTER;
            if (colliderHeight < MIN_COLLIDER_HEIGHT)
            {
                colliderHeight = MIN_COLLIDER_HEIGHT;
            }
            float colliderYCenter = (-1 * colliderHeight / 2) + EXTRA_HEIGHT_ABOVE_CHARACTER;
            collider.size = new Vector3(COLLIDER_FRONT_IN_METERS, colliderHeight, COLLIDER_SIDE_IN_METERS);
            collider.center = new Vector3(0, colliderYCenter, 0);
            return true;
        }

        private bool IsCharacterTooLow(float characterHeight)
        {
            if (characterHeight < MIN_COLLIDER_HEIGHT)
            {
                return true;
            }
            return false;
        }
    }
}
