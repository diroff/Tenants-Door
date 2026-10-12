using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace UI.MainMenu
{
    public class UICharacterBlink : MonoBehaviour
    {
        [SerializeField] private GameObject blinkGameObject;
        [SerializeField] private float interval = 4f;
        [SerializeField] private float duration = 4f;

        private void Start()
        {
            if (blinkGameObject != null)
                BlinkAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid BlinkAsync(CancellationToken cancellationToken)
        {
            blinkGameObject.SetActive(false);

            while (!cancellationToken.IsCancellationRequested)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(interval), cancellationToken: cancellationToken);

                blinkGameObject.SetActive(true);
                await UniTask.Delay(System.TimeSpan.FromSeconds(duration), cancellationToken: cancellationToken);

                blinkGameObject.SetActive(false);
            }
        }
    }
}