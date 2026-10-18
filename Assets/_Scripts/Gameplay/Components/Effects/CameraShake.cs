using DG.Tweening;
using UnityEngine;

namespace Gameplay.Components.Effects
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform _target;

        [Header("Default Shake Settings")]
        [SerializeField, Min(0.01f)] private float _defaultDuration = 0.25f;
        [SerializeField, Min(0f)] private float _defaultStrength = 0.15f;
        [SerializeField, Min(1)] private int _defaultVibrato = 15;

        private Tweener _shakeTween;

        private Vector3 _baseLocalPosition;
        private Vector3 _lastOffset;

        private float _shakeProgress;
        private float _currentStrength;
        private float _seedX;
        private float _seedY;
        private float _initialNoiseX;
        private float _initialNoiseY;

        private int _currentVibrato;
        private bool _isShaking;

        private void Awake()
        {
            if (_target == null)
                _target = transform;

            _baseLocalPosition = _target.localPosition;

            _shakeTween = DOTween.To(
                    () => _shakeProgress,
                    SetShakeProgress,
                    1f,
                    1f)
                .SetEase(Ease.Linear)
                .SetAutoKill(false)
                .SetUpdate(true)
                .OnComplete(HandleShakeCompleted);

            _shakeTween.Pause();
        }

        private void OnDisable()
        {
            StopShake();
        }

        private void OnDestroy()
        {
            _shakeTween?.Kill();
        }

        private void LateUpdate()
        {
            if (_target == null)
                return;

            SyncBasePosition();

            if (!_isShaking)
            {
                _lastOffset = Vector3.zero;
                _target.localPosition = _baseLocalPosition;
                return;
            }

            float progress = Mathf.Clamp01(_shakeProgress);
            float envelope = 1f - progress;
            float noiseTime = progress * _currentVibrato * 0.5f;

            float noiseX = SampleNoise(_seedX, noiseTime) - _initialNoiseX;
            float noiseY = SampleNoise(_seedY, noiseTime) - _initialNoiseY;

            Vector2 offset = new Vector2(noiseX, noiseY);

            if (offset.sqrMagnitude > 1f)
                offset.Normalize();

            _lastOffset = new Vector3(offset.x, offset.y, 0f)
                * (_currentStrength * envelope);

            _target.localPosition = _baseLocalPosition + _lastOffset;
        }

        public void Shake()
        {
            Shake(_defaultDuration, _defaultStrength, _defaultVibrato);
        }

        public void Shake(float duration, float strength, int vibrato = 15)
        {
            if (!isActiveAndEnabled || _target == null || duration <= 0f || strength <= 0f)
                return;

            _currentStrength = strength;
            _currentVibrato = Mathf.Max(1, vibrato);

            _seedX = Random.Range(0f, 1000f);
            _seedY = Random.Range(0f, 1000f);

            _initialNoiseX = SampleNoise(_seedX, 0f);
            _initialNoiseY = SampleNoise(_seedY, 0f);

            _shakeProgress = 0f;
            _isShaking = true;

            _shakeTween.timeScale = 1f / duration;
            _shakeTween.Restart();
        }

        public void StopShake()
        {
            if (_shakeTween != null)
                _shakeTween.Pause();

            _isShaking = false;
            _shakeProgress = 0f;

            RestorePosition();
        }

        private void SetShakeProgress(float progress)
        {
            _shakeProgress = progress;
        }

        private void HandleShakeCompleted()
        {
            _isShaking = false;
            _shakeProgress = 0f;
        }

        private float SampleNoise(float seed, float time)
        {
            return Mathf.PerlinNoise(seed, time) * 2f - 1f;
        }

        private void SyncBasePosition()
        {
            Vector3 expectedPosition = _baseLocalPosition + _lastOffset;
            _baseLocalPosition += _target.localPosition - expectedPosition;
        }

        private void RestorePosition()
        {
            if (_target != null)
            {
                SyncBasePosition();
                _target.localPosition = _baseLocalPosition;
            }

            _lastOffset = Vector3.zero;
        }
    }
}