using DG.Tweening;
using Gameplay.Components.Audio;
using UnityEngine;

namespace Gameplay.Components.Interaction.Doors.Views
{
    public class BaseDoorView : MonoBehaviour
    {
        [Header("Door Target")]
        [SerializeField] private BaseDoor _door;
        [SerializeField] private AudioPlayer _audioPlayer;

        [Header("Animation Settings")]
        [SerializeField] private Transform _doorPivot;
        [SerializeField] private Vector3 _openRotation = new Vector3(0f, 90f, 0f);
        [SerializeField] private float _duration = 1f;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        private Tween _animationTween;

        protected virtual void OnEnable()
        {
            if (_door == null)
                return;

            _door.OnOpeningStarted += HandleOpeningStarted;
        }

        protected virtual void OnDisable()
        {
            if (_door == null)
                return;

            _door.OnOpeningStarted -= HandleOpeningStarted;
            _animationTween?.Kill();
        }

        protected virtual void HandleOpeningStarted()
        {
            AnimateOpening();
            _audioPlayer?.PlayOneShot();
        }

        protected virtual void AnimateOpening()
        {
            _animationTween?.Kill();

            if (_doorPivot == null)
            {
                _door.SetOpened();
                return;
            }

            _animationTween = _doorPivot
                .DOLocalRotate(_openRotation, _duration)
                .SetEase(_ease)
                .SetLink(gameObject)
                .OnComplete(() =>
                {
                    _door.SetOpened();
                });
        }
    }
}