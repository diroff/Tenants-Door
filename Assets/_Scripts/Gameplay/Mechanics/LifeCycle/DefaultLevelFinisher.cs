using Abstractions.Interfaces;
using Abstractions.Mechanics.Lifecycle;
using Cysharp.Threading.Tasks;
using Data.Dialogue;
using Gameplay.Components;
using Gameplay.Components.Level;
using Reflex.Attributes;
using Services;
using System;
using System.Threading;
using UnityEngine;

namespace Gameplay.Infrastructure.Lifecycle.Implementations
{
    public class DefaultLevelFinisher : LevelFinisher
    {
        [Header("Optional Dialogue")]
        [SerializeField] private DialogueComponent _finishDialogue;

        [Header("Settings")]
        [SerializeField] private float _endDuration = 1f;
        [SerializeField] private NextLevelLoader _nextLevelLoader;

        [Inject] private readonly DialogueService _dialogueService;
        [Inject] private readonly InputService _inputService;
        [Inject] private readonly ILevelCompleter _completer;

        private DialogueSO _activeTargetDialogue;

        private void OnEnable()
        {
            if (_completer != null)
                _completer.OnLevelCompleted += HandleLevelCompleted;

            if (_dialogueService != null)
                _dialogueService.OnDialogueEndedWithSO += HandleDialogueEnded;
        }

        private void OnDisable()
        {
            if (_completer != null)
                _completer.OnLevelCompleted -= HandleLevelCompleted;

            if (_dialogueService != null)
                _dialogueService.OnDialogueEndedWithSO -= HandleDialogueEnded;
        }

        private void HandleLevelCompleted(ILevelCompleter completer)
        {
            ExecuteFinishAsync().Forget();
        }

        private void HandleDialogueEnded(DialogueSO dialogue)
        {
            if (_activeTargetDialogue == dialogue)
                _activeTargetDialogue = null;
        }

        protected override void OnBeforeFinish()
        {
            _inputService?.Disable();
        }

        protected override async UniTask OnFinishProcessAsync(CancellationToken cancellationToken)
        {
            if (_finishDialogue != null && _finishDialogue.CanStart())
            {
                _activeTargetDialogue = _finishDialogue.Dialogue;
                _finishDialogue.StartDialogue();

                await UniTask.WaitUntil(() => _activeTargetDialogue == null, cancellationToken: cancellationToken);
            }

            _inputService?.Disable();

            if (_endDuration > 0f)
                await UniTask.Delay(TimeSpan.FromSeconds(_endDuration), cancellationToken: cancellationToken);

            if (_nextLevelLoader != null)
                await _nextLevelLoader.LoadNextStageAsync(cancellationToken);
        }
    }
}