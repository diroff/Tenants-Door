using Reflex.Attributes;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.AudioHandlers
{
    public class UINavigationAudioHandler : MonoBehaviour, ISelectHandler
    {
        [Inject] private AudioEffectsService _audioEffectsService;

        public void OnSelect(BaseEventData eventData)
        {
            _audioEffectsService.PlayUINavigation();
        }
    }
}