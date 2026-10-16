using Abstractions.Interfaces;
using Reflex.Core;
using Reflex.Injectors;
using Services;
using UnityEngine;
using UnityEngine.Audio;

namespace Installers
{
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private string _menuSceneName;
        [SerializeField] private AudioMixer _audioMixer;
        [SerializeField] private AudioMixerGroup _musicMixerGroup;
        [SerializeField] private Material _tvNoiseMaterial;
        [SerializeField] private AudioEffectsService _audioEffectsServicePrefab;

        [Header("Ambient Noise Settings")]
        [SerializeField] private Vector2 _ambientNoiseRange = new Vector2(0.02f, 0.1f);
        [SerializeField] private float _ambientNoiseSpeed = 1.5f;

        public void InstallBindings(ContainerBuilder builder)
        {
            var loadingScreenService = new LoadingScreenService(_tvNoiseMaterial, _ambientNoiseRange, _ambientNoiseSpeed);
            builder.AddSingleton(loadingScreenService, typeof(IService), typeof(LoadingScreenService));

            var sceneManager = new SceneManagerService(_menuSceneName, loadingScreenService);
            builder.AddSingleton(sceneManager, typeof(IService), typeof(SceneManagerService));

            builder.AddSingleton(typeof(InputService), typeof(IService), typeof(InputService));

            var dialogueService = new DialogueService();
            builder.AddSingleton(dialogueService, typeof(IService), typeof(DialogueService));

            builder.AddSingleton(typeof(JsonToFileStorageService), typeof(IStorageService));
            builder.AddSingleton(typeof(SaveLoadService), typeof(IService), typeof(SaveLoadService));

            builder.AddSingleton(_audioMixer, typeof(AudioMixer));
            builder.AddSingleton(_musicMixerGroup, typeof(AudioMixerGroup));

            builder.AddSingleton(typeof(AudioSettingsService), typeof(IService), typeof(AudioSettingsService));
            builder.AddSingleton(typeof(AudioMusicService), typeof(IService), typeof(AudioMusicService));

            var audioEffectsService = Instantiate(_audioEffectsServicePrefab);
            builder.AddSingleton(audioEffectsService, typeof(IService), typeof(AudioEffectsService));

            builder.OnContainerBuilt += container => AttributeInjector.Inject(loadingScreenService, container);
        }
    }
}