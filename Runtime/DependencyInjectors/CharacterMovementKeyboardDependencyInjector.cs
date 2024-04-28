using UnityEngine;

using ReupVirtualTwin.behaviours;
using ReupVirtualTwin.managerInterfaces;

namespace ReupVirtualTwin.dependencyInjectors
{
    public class CharacterMovementKeyboardDependencyInjector : MonoBehaviour
    {
        [SerializeField]
        GameObject characterPositionManager;
        [SerializeField]
        Transform innerCharacterTransform;
        private void Awake()
        {
            CharacterMovementKeyboard characterMovementKeyboard = GetComponent<CharacterMovementKeyboard>();
            characterMovementKeyboard.characterPositionManager = characterPositionManager.GetComponent<ICharacterPositionManager>();
            characterMovementKeyboard.innerCharacterTransform = innerCharacterTransform;
        }
    }
}
