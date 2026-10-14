using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Gameplay.Components.Interaction.Doors
{
    public class ElevatorView : MonoBehaviour
    {
        [SerializeField] private Transform _leftDoor;
        [SerializeField] private Transform _rightDoor;
        [SerializeField] private float _slideDistance = 1.5f;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private Ease _ease = Ease.InOutQuad;

        private Vector3 _leftInitialLocalPosition;
        private Vector3 _rightInitialLocalPosition;
        private Tween _leftTween;
        private Tween _rightTween;

        private void Awake()
        {
            if (_leftDoor != null)
                _leftInitialLocalPosition = _leftDoor.localPosition;

            if (_rightDoor != null)
                _rightInitialLocalPosition = _rightDoor.localPosition;
        }

        private void OnDestroy()
        {
            KillTweens();
        }

        public void Open()
        {
            OpenAsync().Forget();
        }

        public void Close()
        {
            CloseAsync().Forget();
        }

        public async UniTask OpenAsync()
        {
            KillTweens();

            var leftTarget = _leftInitialLocalPosition + Vector3.left * _slideDistance;
            var rightTarget = _rightInitialLocalPosition + Vector3.right * _slideDistance;

            var leftTask = _leftDoor != null
                ? (_leftTween = _leftDoor.DOLocalMove(leftTarget, _duration).SetEase(_ease)).ToUniTask()
                : UniTask.CompletedTask;

            var rightTask = _rightDoor != null
                ? (_rightTween = _rightDoor.DOLocalMove(rightTarget, _duration).SetEase(_ease)).ToUniTask()
                : UniTask.CompletedTask;

            await UniTask.WhenAll(leftTask, rightTask);
        }

        public async UniTask CloseAsync()
        {
            KillTweens();

            var leftTask = _leftDoor != null
                ? (_leftTween = _leftDoor.DOLocalMove(_leftInitialLocalPosition, _duration).SetEase(_ease)).ToUniTask()
                : UniTask.CompletedTask;

            var rightTask = _rightDoor != null
                ? (_rightTween = _rightDoor.DOLocalMove(_rightInitialLocalPosition, _duration).SetEase(_ease)).ToUniTask()
                : UniTask.CompletedTask;

            await UniTask.WhenAll(leftTask, rightTask);
        }

        private void KillTweens()
        {
            _leftTween?.Kill();
            _rightTween?.Kill();
        }
    }
}