using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using Services;
using System.Threading;

namespace UI.MainMenu
{
    public class UILastLevelLoaderButton : UIMainMenuButton
    {
        [Inject] private SceneManagerService _sceneManager;
        [Inject] private SaveLoadService _saveLoadService;

        protected override async UniTask OnClickAsync(CancellationToken ct)
        {
            await base.OnClickAsync(ct);

            SetInteractable(false);

            var levelName = _saveLoadService.Data.Progress.LastReachedLevelName;
            await _sceneManager.LoadSceneAsync(levelName);

            SetInteractable(true);
        }
    }
}