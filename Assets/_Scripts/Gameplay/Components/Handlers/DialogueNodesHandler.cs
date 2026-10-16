using Data.Dialogue;
using Reflex.Attributes;
using Services;
using UnityEngine;

namespace Gameplay.Components.Handlers
{
    public class DialogueNodesHandler : MonoBehaviour
    {
        [Inject] private DialogueService _dialogueService;
        [Inject] private AudioEffectsService _audioEffectsService;

        private void OnEnable()
        {
            _dialogueService.OnDialogueStarted += OnDialogueFinished;
            _dialogueService.OnNodeStarted += OnNodeStarted;
        }

        private void OnDisable()
        {
            _dialogueService.OnDialogueStarted -= OnDialogueFinished;
            _dialogueService.OnNodeStarted -= OnNodeStarted;
        }

        private void OnDialogueFinished()
        {
            _audioEffectsService.PlayUICancel();
        }

        private void OnNodeStarted(DialogueNode node)
        {
            _audioEffectsService.PlayUIConfirm();
        }
    }
}