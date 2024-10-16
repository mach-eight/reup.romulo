using UnityEngine;

using ReupVirtualTwin.behaviours;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class CharacterMovementKeyboardDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        Transform innerCharacterTransform;
        private void Awake()
        {
            CharacterMovementKeyboard characterMovementKeyboard = GetComponent<CharacterMovementKeyboard>();
            characterMovementKeyboard.innerCharacterTransform = innerCharacterTransform;
        }
    }
}
