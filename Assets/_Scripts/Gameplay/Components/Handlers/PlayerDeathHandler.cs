using Abstractions.Components;
using Cysharp.Threading.Tasks;
using Gameplay.Mechanics.Player;
using Reflex.Attributes;
using Services;
using UnityEngine;

namespace Gameplay.Components.Handlers
{
    public class PlayerDeathHandler : BaseDeathHandler
    {
        [SerializeField] private Vector3 _targetRotation;

        [Inject] private Player _player;
        [Inject] private InputService _inputService;
        [Inject] private SceneManagerService _sceneManagerService;

        private Transform _playerView;

        private void Awake()
        {
            if (_health == null && _player != null)
                _health = _player.GetComponent<HealthComponent>();

            _playerView = _player.transform.GetChild(0);
        }

        protected override void HandleDeath(GameObject dead, GameObject killer)
        {
            base.HandleDeath(dead, killer);

            _inputService?.EnableUI();

            _player.transform.Rotate(_targetRotation);
            _sceneManagerService.ReloadSceneAsync().Forget();
        }

        protected override void HandleRevive(GameObject sender)
        {
            base.HandleRevive(sender);

            _inputService?.EnableGameplay();

        }
    }
}