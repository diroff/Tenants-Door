using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Services;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace UI
{
    public class UIService : MonoBehaviour
    {
        [Header("Initial Panel (Optional)")]
        [SerializeField] private UIPanel _initialPanel;

        [Inject] private InputService _input;
        [Inject] private AudioEffectsService _audioEffectsService;

        private readonly Stack<UIPanel> _history = new();
        private bool _isTransitioning;
        private CancellationTokenSource _cts;

        private void Awake()
        {
            _cts = new CancellationTokenSource();
        }

        private void Start()
        {
            _input.EnableUI();
            SubscribeInput();

            if (_initialPanel != null)
                OpenPanelAsync(_initialPanel).Forget();
        }

        private void OnDestroy()
        {
            UnsubscribeInput();
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void SubscribeInput()
        {
            if (_input != null)
                _input.Actions.UI.Cancel.performed += OnCancelPerformed;
        }

        private void UnsubscribeInput()
        {
            if (_input != null)
                _input.Actions.UI.Cancel.performed -= OnCancelPerformed;
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (_isTransitioning || _history.Count <= 1)
                return;

            GoBackAsync().Forget();
        }

        public async UniTask OpenPanelAsync(UIPanel targetPanel)
        {
            if (_isTransitioning || targetPanel == null)
                return;

            _isTransitioning = true;
            EventSystem.current?.SetSelectedGameObject(null);

            if (_history.Count > 0)
            {
                UIPanel currentPanel = _history.Peek();

                if (targetPanel.IsPopup)
                    currentPanel.SetInteractable(false);
                else
                    await currentPanel.HideAsync(_cts.Token);
            }

            _history.Push(targetPanel);
            await targetPanel.ShowAsync(_cts.Token);

            _isTransitioning = false;
        }

        public async UniTask GoBackAsync()
        {
            if (_isTransitioning || _history.Count <= 1)
                return;

            _isTransitioning = true;
            EventSystem.current?.SetSelectedGameObject(null);
            _audioEffectsService.PlayUICancel();

            var currentPanel = _history.Pop();
            await currentPanel.HideAsync(_cts.Token);

            var previousPanel = _history.Peek();

            if (currentPanel.IsPopup)
            {
                previousPanel.SetInteractable(true);
                previousPanel.FocusFirstSelectable();
            }
            else
                await previousPanel.ShowAsync(_cts.Token);

            _isTransitioning = false;
        }
    }
}