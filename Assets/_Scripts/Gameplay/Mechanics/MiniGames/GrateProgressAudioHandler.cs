using Gameplay.Components.Audio;
using UnityEngine;

namespace Gameplay.Mechanics.MiniGames
{
    public class GrateProgressAudioHandler : MonoBehaviour
    {
        [SerializeField] private GrateMiniGame _grateMiniGame;
        [SerializeField] private AudioLoopPlayer _progressAudioPlayer;

        private void OnEnable()
        {
            if (_grateMiniGame == null || _grateMiniGame.QteComponent == null)
                return;

            _grateMiniGame.QteComponent.OnProgressChanged += HandleProgressChanged;
            HandleProgressChanged(_grateMiniGame.QteComponent.Progress);
        }

        private void OnDisable()
        {
            if (_grateMiniGame != null && _grateMiniGame.QteComponent != null)
                _grateMiniGame.QteComponent.OnProgressChanged -= HandleProgressChanged;

            _progressAudioPlayer?.StopLoop();
        }

        private void HandleProgressChanged(float progress)
        {
            if (_progressAudioPlayer == null)
                return;

            if (progress <= 0f || progress >= 1f)
            {
                _progressAudioPlayer.StopLoop();
                return;
            }

            _progressAudioPlayer.StartLoop();
        }
    }
}