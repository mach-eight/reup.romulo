using UnityEngine;

using ReupVirtualTwin.inputs;
using ReupVirtualTwin.enums;
using ReupVirtualTwin.managerInterfaces;
using Zenject;

namespace ReupVirtualTwin.behaviours
{
    public class ChangeHeight : MonoBehaviour
    {
        public static float CHANGE_SPEED_M_PER_SECOND = 1;

        private InputProvider inputProvider;
        private bool _movingHeight = false;
        private IMediator _mediator;
        public IMediator mediator { set => _mediator = value; }

        [Inject]
        public void Init(InputProvider inputProvider)
        {
            this.inputProvider = inputProvider;
        }

        private void Update()
        {
            float changeHeightInput = inputProvider.ChangeHeightInput();
            if (changeHeightInput != 0)
            {
                _movingHeight = true;
                float deltaHeight = changeHeightInput * CHANGE_SPEED_M_PER_SECOND * Time.deltaTime;
                _mediator.Notify(ReupEvent.addToCharacterHeight, deltaHeight);
            }
            else if (_movingHeight && changeHeightInput == 0)
            {
                _movingHeight = false;
                _mediator.Notify(ReupEvent.setCharacterHeight);
            }
        }
    }
}
