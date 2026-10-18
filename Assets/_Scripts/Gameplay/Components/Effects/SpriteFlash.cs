using DG.Tweening;
using UnityEngine;

namespace Gameplay.Components.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteFlash : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpriteRenderer _spriteRenderer;

        [Header("Flash Settings")]
        [SerializeField, Min(0.01f)] private float _defaultDuration = 0.2f;
        [SerializeField, Range(0f, 1f)] private float _defaultStrength = 1f;

        private static readonly int FlashAmountProperty = Shader.PropertyToID("_FlashAmount");

        private MaterialPropertyBlock _propertyBlock;
        private Tweener _flashTween;

        private float _flashProgress;
        private float _currentStrength;

        private void Awake()
        {
            if (_spriteRenderer == null)
                _spriteRenderer = GetComponent<SpriteRenderer>();

            _propertyBlock = new MaterialPropertyBlock();

            SetFlashAmount(0f);

            _flashTween = DOTween.To(() => _flashProgress, SetFlashProgress, 1f, 1f)
                .SetEase(Ease.Linear)
                .SetAutoKill(false)
                .SetUpdate(true)
                .OnComplete(HandleFlashCompleted);

            _flashTween.Pause();
        }

        private void OnDisable()
        {
            StopFlash();
        }

        private void OnDestroy()
        {
            _flashTween?.Kill();
        }

        public void Flash()
        {
            Flash(_defaultDuration, _defaultStrength);
        }

        public void Flash(float duration, float strength)
        {
            if (!isActiveAndEnabled || _spriteRenderer == null)
                return;

            if (duration <= 0f || strength <= 0f)
            {
                StopFlash();
                return;
            }

            _currentStrength = Mathf.Clamp01(strength);
            _flashProgress = 0f;

            _flashTween.timeScale = 1f / duration;
            _flashTween.Restart();
        }

        public void StopFlash()
        {
            _flashTween?.Pause();

            _flashProgress = 0f;
            SetFlashAmount(0f);
        }

        private void SetFlashProgress(float progress)
        {
            _flashProgress = progress;

            float amount = Mathf.Sin(progress * Mathf.PI) * _currentStrength;
            SetFlashAmount(amount);
        }

        private void SetFlashAmount(float amount)
        {
            if (_spriteRenderer == null)
                return;

            _spriteRenderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetFloat(FlashAmountProperty, amount);
            _spriteRenderer.SetPropertyBlock(_propertyBlock);
        }

        private void HandleFlashCompleted()
        {
            _flashProgress = 0f;
            SetFlashAmount(0f);
        }
    }
}