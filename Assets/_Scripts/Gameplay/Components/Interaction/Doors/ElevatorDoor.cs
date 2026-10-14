using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Services;
using UnityEngine;

namespace Gameplay.Components.Interaction.Doors
{
    public class ElevatorDoor : BaseDoor
    {
        [SerializeField] private ElevatorView _view;

        [Inject] private InputService _input;

        public void Call()
        {
            IsInteractable = true;
            SetOpened();
            OnCall().Forget();
        }

        private async UniTask OnCall()
        {
            _input.EnableUI();

            await _view.OpenAsync();

            SetOpened();
        }
    }
}