using Abstractions.Interfaces;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public class SceneManagerService : IService
    {
        private readonly string _menuSceneName;

        public SceneManagerService(string menuSceneName)
        {
            _menuSceneName = menuSceneName;
        }

        public UniTask Initialize()
        {
            return UniTask.CompletedTask;
        }

        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }

        public void LoadScene(int index)
        {
            SceneManager.LoadScene(index);
        }

        public async UniTask LoadSceneAsync(string name)
        {
            await SceneManager.LoadSceneAsync(name);
        }

        public async UniTask LoadSceneAsync(int index)
        {
            await SceneManager.LoadSceneAsync(index);
        }

        public void LoadMenuScene()
        {
            LoadScene(_menuSceneName);
        }

        public UniTask LoadMenuSceneAsync()
        {
            return LoadSceneAsync(_menuSceneName);
        }

        public void ReloadScene()
        {
            LoadScene(GetActiveScene());
        }

        public async UniTask ReloadSceneAsync()
        {
            await LoadSceneAsync(GetActiveScene());
        }

        private int GetActiveScene()
        {
            return SceneManager.GetActiveScene().buildIndex;
        }

        public void QuitGame()
        {
            #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
            #else
                        Application.Quit();
            #endif
        }
    }
}