using Abstractions.Interfaces;
using Cysharp.Threading.Tasks;
using Data.Dialogue;
using System;

namespace Services
{
    public class DialogueService : IService
    {
        public event Action<DialogueNode> OnNodeStarted;
        public event Action OnDialogueStarted;
        public event Action OnDialogueEnded;
        public event Action<DialogueSO> OnDialogueEndedWithSO;

        public bool IsActive { get; private set; }
        public DialogueNode CurrentNode => _currentDialogue.Nodes[_currentIndex];

        private DialogueSO _currentDialogue;
        private int _currentIndex;

        public async UniTask Initialize()
        {
            await UniTask.CompletedTask;
        }

        public void StartDialogue(DialogueSO dialogue)
        {
            if (dialogue == null || dialogue.Nodes.Count == 0)
                return;

            _currentDialogue = dialogue;
            _currentIndex = 0;
            IsActive = true;
            OnDialogueStarted?.Invoke();

            ShowCurrentNode();
        }

        public void Advance()
        {
            if (!IsActive)
                return;

            _currentIndex++;

            if (_currentIndex < _currentDialogue.Nodes.Count)
                ShowCurrentNode();
            else
                EndDialogue();
        }

        public void EndDialogue()
        {
            if (!IsActive)
                return;

            IsActive = false;

            var endedDialogue = _currentDialogue;
            OnDialogueEnded?.Invoke();
            OnDialogueEndedWithSO?.Invoke(endedDialogue);

            _currentDialogue = null;
        }

        private void ShowCurrentNode()
        {
            OnNodeStarted?.Invoke(CurrentNode);
        }
    }
}