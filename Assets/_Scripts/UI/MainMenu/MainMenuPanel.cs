using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuPanel : UIPanel
    {
        [Header("Main Menu Buttons")]
        [SerializeField] private Button _continueButton;
        [SerializeField] private Button _newGameButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;

        [Header("Target Panels")]
        [SerializeField] private UIPanel _settingsPanel;
        [SerializeField] private UIPanel _exitConfirmationPopup;

        [Inject] private UIService _uiService;

        private void Start()
        {
            Initialize(true);
        }

        public void Initialize(bool hasSaveData)
        {
            _continueButton.interactable = hasSaveData;

            _settingsButton.onClick.AddListener(OnSettingsClicked);
            _exitButton.onClick.AddListener(OnExitClicked);
        }

        private void OnSettingsClicked()
        {
            _uiService.OpenPanelAsync(_settingsPanel).Forget();
        }

        private void OnExitClicked()
        {
            _uiService.OpenPanelAsync(_exitConfirmationPopup).Forget();
        }
    }
}