using Reflex.Attributes;
using Services;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Abstractions.UI
{
    [RequireComponent(typeof(Selectable))]
    public abstract class UIValueButtonBase : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField, Min(0f)] private float _initialRepeatDelay = 0.35f;
        [SerializeField, Min(0.01f)] private float _repeatInterval = 0.1f;

        [Inject] private InputService _inputService;
        [Inject] private AudioEffectsService _audioEffectsService;

        private Selectable _selectable;
        private bool _isSelected;
        private int _heldDirection;
        private float _repeatTimer;

        protected virtual void Awake()
        {
            _selectable = GetComponent<Selectable>();
            ConfigureNavigation();
        }

        protected virtual void OnEnable()
        {
            if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                _isSelected = true;
        }

        protected virtual void OnDisable()
        {
            _isSelected = false;
            _heldDirection = 0;
            _repeatTimer = 0f;
        }

        protected virtual void Update()
        {
            if (!_isSelected || _inputService == null)
                return;

            var navigateAction = _inputService.Actions.UI.Navigate;

            if (!navigateAction.enabled)
                return;

            float horizontal = navigateAction.ReadValue<Vector2>().x;
            int direction = horizontal > 0.5f ? 1 : horizontal < -0.5f ? -1 : 0;

            if (direction == 0)
            {
                _heldDirection = 0;
                _repeatTimer = 0f;
                return;
            }

            if (direction != _heldDirection)
            {
                _heldDirection = direction;
                _repeatTimer = _initialRepeatDelay;
                TryApplyValueStep(direction);
                return;
            }

            _repeatTimer -= Time.unscaledDeltaTime;

            if (_repeatTimer <= 0f)
            {
                TryApplyValueStep(direction);
                _repeatTimer = Mathf.Max(0.01f, _repeatInterval);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            _isSelected = true;
            _heldDirection = 0;
            _repeatTimer = 0f;
        }

        public void OnDeselect(BaseEventData eventData)
        {
            _isSelected = false;
            _heldDirection = 0;
            _repeatTimer = 0f;
        }

        protected abstract bool OnValueStep(int direction);

        private void TryApplyValueStep(int direction)
        {
            if (OnValueStep(direction))
                _audioEffectsService?.PlayUINavigation();
        }

        private void ConfigureNavigation()
        {
            Navigation navigation = _selectable.navigation;

            if (navigation.mode == Navigation.Mode.None)
                return;

            Selectable selectOnUp = _selectable.FindSelectableOnUp();
            Selectable selectOnDown = _selectable.FindSelectableOnDown();

            navigation.mode = Navigation.Mode.Explicit;
            navigation.selectOnUp = selectOnUp;
            navigation.selectOnDown = selectOnDown;
            navigation.selectOnLeft = null;
            navigation.selectOnRight = null;

            _selectable.navigation = navigation;
        }
    }
}