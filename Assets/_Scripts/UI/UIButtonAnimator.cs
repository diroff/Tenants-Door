using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class UIButtonAnimator : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
    {
        [SerializeField] private float _selectedScale = 1.08f;
        [SerializeField] private float _pressScale = 0.95f;
        [SerializeField] private float _duration = 0.15f;
        [SerializeField] private Ease _easeIn = Ease.OutQuad;
        [SerializeField] private Ease _easeOut = Ease.OutBack;

        private Vector3 _initialScale;
        private Sequence _animationSequence;

        private void Awake()
        {
            _initialScale = transform.localScale;
        }

        private void OnDisable()
        {
            _animationSequence?.Kill();
            transform.localScale = _initialScale;
        }

        private void OnDestroy()
        {
            _animationSequence?.Kill();
            transform.DOKill();
        }

        public void OnSelect(BaseEventData eventData)
        {
            AnimateScale(_selectedScale, _duration, _easeOut);
        }

        public void OnDeselect(BaseEventData eventData)
        {
            AnimateScale(1f, _duration, _easeIn);
        }

        public void OnSubmit(BaseEventData eventData)
        {
            _animationSequence?.Kill();
            _animationSequence = DOTween.Sequence()
                .Append(transform.DOScale(_initialScale * _pressScale, 0.08f).SetEase(_easeIn))
                .Append(transform.DOScale(_initialScale * _selectedScale, 0.1f).SetEase(_easeOut))
                .Play();
        }

        private void AnimateScale(float targetScaleMultiplier, float duration, Ease ease)
        {
            _animationSequence?.Kill();
            _animationSequence = DOTween.Sequence()
                .Append(transform.DOScale(_initialScale * targetScaleMultiplier, duration).SetEase(ease))
                .Play();
        }
    }
}