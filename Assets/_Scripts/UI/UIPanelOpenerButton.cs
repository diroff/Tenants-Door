using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UIPanelOpenerButton : MonoBehaviour
    {
        [SerializeField] private UIPanel _panelToOpen;

        [Inject] private UIService _uiService;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _uiService.OpenPanelAsync(_panelToOpen).Forget();
        }
    }
}