using Abstractions.Components;
using Gameplay.Components.Effects;
using Reflex.Attributes;
using UnityEngine;

namespace Gameplay.Mechanics.Player
{
    public class PlayerHitsHandler : MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField] private CameraShake _cameraShake;
        [SerializeField] private SpriteFlash _spriteFlash;

        [Inject] private Player _player;

        private HealthComponent _health;

        private void Awake()
        {
            _health = _player.GetComponentInChildren<HealthComponent>();
        }

        private void OnEnable()
        {
            _health.OnDamage += HandleDamage;
        }

        private void OnDisable()
        {
            _health.OnDamage -= HandleDamage;
        }

        private void HandleDamage(GameObject sender)
        {
            _cameraShake?.Shake();
            _spriteFlash?.Flash();
        }
    }
}