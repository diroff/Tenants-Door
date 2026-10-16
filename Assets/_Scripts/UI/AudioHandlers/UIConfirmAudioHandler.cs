using Reflex.Attributes;
using Services;
using UnityEngine;
using UnityEngine.UI;

namespace UI.AudioHandlers
{
    [RequireComponent(typeof(Button))]
    public class UIConfirmAudioHandler : MonoBehaviour
    {
        [Inject] private AudioEffectsService _audioEffectsService;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            _audioEffectsService.PlayUIConfirm();
        }
    }
}