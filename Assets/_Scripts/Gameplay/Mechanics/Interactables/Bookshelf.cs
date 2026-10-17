using Cysharp.Threading.Tasks;
using DG.Tweening;
using Reflex.Attributes;
using Services;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace Gameplay.Mechanics.Interactables
{
    public class Bookshelf : MonoBehaviour
    {
        [Header("Shelf Movement")]
        [SerializeField] private Transform _shelfTransform;
        [SerializeField] private Vector2 _targetPosition;
        [SerializeField] private float _moveDuration = 1.5f;
        [SerializeField] private Ease _moveEase = Ease.OutQuad;

        [Header("Book Settings")]
        [SerializeField] private GameObject _bookObject;
        [SerializeField] private Vector3 _insertedScaleMultiplier = new Vector3(1.3f, 1.3f, 1f);
        [SerializeField] private float _bookScaleDuration = 0.35f;
        [SerializeField] private float _bookEndDelay = 0.25f;
        [SerializeField] private Ease _bookScaleEase = Ease.OutBack;

        [Header("Events")]
        [SerializeField] private UnityEvent _onAnimationStarted;
        [SerializeField] private UnityEvent _onAnimationCompleted;

        [Inject] private InputService _input;

        private Vector3 _originalBookScale;
        private Sequence _activeSequence;

        private void Awake()
        {
            if (_shelfTransform == null)
                _shelfTransform = transform;

            _originalBookScale = _bookObject.transform.localScale;
            _bookObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _activeSequence?.Kill();
        }

        public void MoveBookshelf()
        {
            PlayUnlockAnimationAsync(destroyCancellationToken).Forget();
        }

        public async UniTask PlayUnlockAnimationAsync(CancellationToken cancellationToken = default)
        {
            _activeSequence?.Kill();
            _input.Disable();

            _bookObject.transform.localScale = Vector3.Scale(_originalBookScale, _insertedScaleMultiplier);
            _bookObject.SetActive(true);

            _activeSequence = DOTween.Sequence()
                .Append(_bookObject.transform
                    .DOScale(_originalBookScale, _bookScaleDuration)
                    .SetEase(_bookScaleEase))
                .AppendInterval(_bookEndDelay)
                .AppendCallback(() => _onAnimationStarted?.Invoke())
                .Append(_shelfTransform
                    .DOMove(_targetPosition, _moveDuration)
                    .SetEase(_moveEase));

            await _activeSequence
                .WithCancellation(cancellationToken)
                .SuppressCancellationThrow();

            _onAnimationCompleted?.Invoke();
            _input.EnableGameplay();
        }
    }
}