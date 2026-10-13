using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour
    {
        [Header("Focus Settings")]
        [SerializeField] private Selectable _firstSelected;

        [Header("Animation Settings")]
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;

        [Header("Behavior")]
        [SerializeField] private bool _isPopup = false;

        protected CanvasGroup _canvasGroup;
        private Sequence _fadeSequence;
        private Selectable[] _cachedSelectables;

        public bool IsPopup => _isPopup;
        public Selectable FirstSelected => _firstSelected;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _cachedSelectables = GetComponentsInChildren<Selectable>(true);
        }

        protected virtual void OnDestroy()
        {
            _fadeSequence?.Kill();
        }

        public virtual async UniTask ShowAsync(CancellationToken cancellationToken = default)
        {
            gameObject.SetActive(true);
            SetInteractable(false);

            _fadeSequence?.Kill();
            _canvasGroup.alpha = 0f;

            _fadeSequence = DOTween.Sequence()
                .Append(_canvasGroup.DOFade(1f, _fadeDuration).SetEase(_fadeEase));

            await _fadeSequence
                .WithCancellation(cancellationToken)
                .SuppressCancellationThrow();

            SetInteractable(true);
            FocusFirstSelectable();
        }

        public virtual async UniTask HideAsync(CancellationToken cancellationToken = default)
        {
            SetInteractable(false);
            DeselectAllButtons();

            _fadeSequence?.Kill();
            _fadeSequence = DOTween.Sequence()
                .Append(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(_fadeEase));

            await _fadeSequence
                .WithCancellation(cancellationToken)
                .SuppressCancellationThrow();

            gameObject.SetActive(false);
        }

        public void SetInteractable(bool interactable)
        {
            _canvasGroup.interactable = interactable;
            _canvasGroup.blocksRaycasts = interactable;

            if (!interactable)
                DeselectAllButtons();
        }

        private void DeselectAllButtons()
        {
            if (_cachedSelectables == null)
                return;

            BaseEventData eventData = new BaseEventData(EventSystem.current);

            foreach (var selectable in _cachedSelectables)
            {
                if (selectable != null && selectable.gameObject.activeInHierarchy)
                    ExecuteEvents.Execute(selectable.gameObject, eventData, ExecuteEvents.deselectHandler);
            }
        }

        public void FocusFirstSelectable()
        {
            if (_firstSelected != null)
                Select(_firstSelected);
            else
                SelectNextSelectable();
        }

        public void Select(Selectable selectable)
        {
            if (selectable == null || !selectable.IsInteractable() || !selectable.gameObject.activeInHierarchy)
            {
                SelectNextSelectable(selectable);
                return;
            }

            selectable.Select();
        }

        protected void SelectNextSelectable(Selectable afterSelectable = null)
        {
            if (_cachedSelectables == null || _cachedSelectables.Length == 0)
                return;

            bool foundStart = (afterSelectable == null);

            foreach (var selectable in _cachedSelectables)
            {
                if (selectable == null) continue;

                if (!foundStart)
                {
                    if (selectable == afterSelectable)
                        foundStart = true;
                    continue;
                }

                if (selectable.gameObject.activeInHierarchy && selectable.IsInteractable())
                {
                    selectable.Select();
                    return;
                }
            }

            foreach (var selectable in _cachedSelectables)
            {
                if (selectable == null) continue;

                if (selectable == afterSelectable)
                    break;

                if (selectable.gameObject.activeInHierarchy && selectable.IsInteractable())
                {
                    selectable.Select();
                    return;
                }
            }
        }
    }
}