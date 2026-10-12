using Cysharp.Threading.Tasks;
using Reflex.Attributes;
using UI.MainMenu;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Button))]
    public class UIBackButton : MonoBehaviour
    {
        [Inject] private UIService _uiService;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(OnClick);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        private void OnClick()
        {
            _uiService?.GoBackAsync().Forget();
        }
    }
}