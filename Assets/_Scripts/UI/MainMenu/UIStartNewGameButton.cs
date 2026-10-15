using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Services;
using System.Threading;
using UnityEngine;

namespace UI.MainMenu
{
    public class UIStartNewGameButton : UIMainMenuButton
    {
        [SerializeField] private string _levelName;

        [Inject] private SceneManagerService _sceneManager;

        protected override async UniTask OnClickAsync(CancellationToken ct)
        {
            await base.OnClickAsync(ct);

            SetInteractable(false);
            await _sceneManager.LoadSceneAsync(_levelName);

            SetInteractable(true);
        }
    }
}