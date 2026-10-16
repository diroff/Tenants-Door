using Abstractions.Components;
using Gameplay.Components.Audio;
using UnityEngine;

namespace Gameplay.Mechanics.Player
{
    [RequireComponent(typeof(AudioLoopPlayer))]
    public class PlayerFootstepAudioHandler : MonoBehaviour
    {
        private MovementComponent _movement;
        private JumpComponent _jump;
        private AudioLoopPlayer _footstepAudio;
        private HealthComponent _health;

        private bool _isMoving;
        private bool _isJumping;
        private bool _isDead;
        private bool _isFootstepsPlaying;

        private void Awake()
        {
            _movement = GetComponentInParent<MovementComponent>();
            _jump = GetComponentInParent<JumpComponent>();
            _health = GetComponentInParent<HealthComponent>();
            _footstepAudio = GetComponent<AudioLoopPlayer>();
        }

        private void OnEnable()
        {
            _movement.OnMoveStarted += HandleMoveStarted;
            _movement.OnMoveStopped += HandleMoveStopped;
            _jump.JumpStarted += HandleJumpStarted;
            _jump.Landed += HandleLanded;

            if (_health != null)
            {
                _health.OnDied += HandleDeath;
                _health.OnRevived += HandleRevive;
            }

            _isMoving = _movement.IsMoving;
            _isJumping = !_jump.IsGrounded();

            UpdateFootstepState();
        }

        private void Update()
        {
            UpdateFootstepState();
        }

        private void OnDisable()
        {
            _movement.OnMoveStarted -= HandleMoveStarted;
            _movement.OnMoveStopped -= HandleMoveStopped;
            _jump.JumpStarted -= HandleJumpStarted;
            _jump.Landed -= HandleLanded;

            if (_health != null)
            {
                _health.OnDied -= HandleDeath;
                _health.OnRevived -= HandleRevive;
            }

            _footstepAudio.StopLoop();
            _isFootstepsPlaying = false;
        }

        private void HandleMoveStarted(Vector3 direction)
        {
            _isMoving = true;
            UpdateFootstepState();
        }

        private void HandleMoveStopped()
        {
            _isMoving = false;
            UpdateFootstepState();
        }

        private void HandleJumpStarted()
        {
            _isJumping = true;
            UpdateFootstepState();
        }

        private void HandleLanded()
        {
            _isJumping = false;
            UpdateFootstepState();
        }

        private void HandleDeath(GameObject dead, GameObject killer)
        {
            _isDead = true;
            UpdateFootstepState();
        }

        private void HandleRevive(GameObject sender)
        {
            _isDead = false;
            UpdateFootstepState();
        }

        private void UpdateFootstepState()
        {
            bool shouldPlay = _isMoving && !_isJumping && !_isDead && _jump.IsGrounded();

            if (shouldPlay == _isFootstepsPlaying)
                return;

            _isFootstepsPlaying = shouldPlay;

            if (shouldPlay)
                _footstepAudio.StartLoop();
            else
                _footstepAudio.StopLoop();
        }
    }
}