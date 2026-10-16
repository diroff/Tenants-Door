using Abstractions.Components;
using Gameplay.Components.Audio;
using UnityEngine;

namespace Gameplay.Mechanics.Player
{
    public class PlayerJumpAudioHandler : MonoBehaviour
    {
        [SerializeField] private AudioPlayer _jumpStartAudio;
        [SerializeField] private AudioPlayer _landingAudio;

        private JumpComponent _jump;

        private void Awake()
        {
            _jump = GetComponentInParent<JumpComponent>();
        }

        private void OnEnable()
        {
            _jump.JumpStarted += HandleJumpStarted;
            _jump.Landed += HandleLanded;
        }

        private void OnDisable()
        {
            _jump.JumpStarted -= HandleJumpStarted;
            _jump.Landed -= HandleLanded;
        }

        private void HandleJumpStarted()
        {
            if (_jumpStartAudio != null)
                _jumpStartAudio.PlayOneShot();
        }

        private void HandleLanded()
        {
            if (_landingAudio != null)
                _landingAudio.PlayOneShot();
        }
    }
}