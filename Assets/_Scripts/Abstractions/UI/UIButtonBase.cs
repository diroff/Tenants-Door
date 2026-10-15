using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Abstractions.UI
{
    public abstract class UIButtonBase : MonoBehaviour
    {
        protected Button Button { get; private set; }

        private CancellationTokenSource _cts;

        protected virtual void Awake()
        {
            Button = GetComponent<Button>();
            _cts = new CancellationTokenSource();
        }

        protected virtual void OnEnable()
        {
            Button.onClick.AddListener(InvokeClick);
        }

        protected virtual void OnDisable()
        {
            Button.onClick.RemoveListener(InvokeClick);
        }

        protected virtual void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void InvokeClick()
        {
            OnClickAsync(_cts.Token).Forget();
        }

        protected virtual UniTask OnClickAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }
        protected void SetInteractable(bool value)
        {
            if (Button == null)
                return;

            Button.interactable = value;
        }
    }
}