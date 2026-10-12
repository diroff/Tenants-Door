using Reflex.Core;
using UI;
using UnityEngine;

namespace Installers
{
    public class MainMenuInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private UIService _uiService;

        public void InstallBindings(ContainerBuilder containerBuilder)
        {
            containerBuilder.AddSingleton(_uiService);
        }
    }
}