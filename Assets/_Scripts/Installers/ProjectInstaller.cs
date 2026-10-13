using Abstractions.Interfaces;
using Reflex.Core;
using Services;
using UnityEngine;
using UnityEngine.Audio;

namespace Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private string _menuSceneName;
        [SerializeField] private AudioMixer _audioMixer;

        public void InstallBindings(ContainerBuilder builder)
        {
            var sceneManager = new SceneManagerService(_menuSceneName);
            builder.AddSingleton(sceneManager, typeof(IService), typeof(SceneManagerService));
            builder.AddSingleton(typeof(InputService), typeof(IService), typeof(InputService));

            var dialogueService = new DialogueService();
            builder.AddSingleton(dialogueService, typeof(IService), typeof(DialogueService));

            builder.AddSingleton(typeof(JsonToFileStorageService), typeof(IStorageService));
            builder.AddSingleton(typeof(SaveLoadService), typeof(IService), typeof(SaveLoadService));

            builder.AddSingleton(_audioMixer, typeof(AudioMixer));
            builder.AddSingleton(typeof(AudioSettingsService), typeof(IService), typeof(AudioSettingsService));
        }
    }
}