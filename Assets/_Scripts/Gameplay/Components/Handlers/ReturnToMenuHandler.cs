using Cysharp.Threading.Tasks;
using Gameplay.Components.Input;
using Reflex.Attributes;
using Services;
using UnityEngine;

namespace Gameplay.Components.Handlers
{
    public class ReturnToMenuHandler : MonoBehaviour
    {
        [SerializeField] private HoldInputDetector _holdDetector;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Inject] private SceneManagerService _sceneManagerService;
        [Inject] private AudioEffectsService _audioEffectsService;

        private bool _isExiting;

        private void OnEnable()
        {
            if (_holdDetector != null)
            {
                _holdDetector.OnProgressChanged += HandleProgressChanged;
                _holdDetector.OnCompleted += HandleHoldCompleted;
            }

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = 0f;
            }
        }

        private void OnDisable()
        {
            if (_holdDetector != null)
            {
                _holdDetector.OnProgressChanged -= HandleProgressChanged;
                _holdDetector.OnCompleted -= HandleHoldCompleted;
            }
        }

        private void HandleProgressChanged(float progress)
        {
            if (_isExiting) return;

            if (_canvasGroup != null)
            {
                _canvasGroup.alpha = progress;
            }
        }

        private void HandleHoldCompleted()
        {
            if (_isExiting)
                return;

            _isExiting = true;

            _audioEffectsService.PlayUICancel();
            LoadMenuAsync().Forget();
        }

        private async UniTaskVoid LoadMenuAsync()
        {
            await _sceneManagerService.LoadMenuSceneAsync();
        }
    }
}