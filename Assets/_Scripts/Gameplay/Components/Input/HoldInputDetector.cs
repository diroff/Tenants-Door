using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gameplay.Components.Input
{
    public class HoldInputDetector : MonoBehaviour
    {
        [SerializeField] private InputActionReference _inputActionReference;
        [SerializeField] private float _requiredHoldTime = 1.5f;

        public event Action<float> OnProgressChanged;
        public event Action OnCompleted;
        public event Action OnCancelled;

        private bool _isHolding;
        private bool _isCompleted;
        private float _currentHoldTime;

        private void OnEnable()
        {
            if (_inputActionReference == null || _inputActionReference.action == null)
                return;

            _inputActionReference.action.Enable();
            _inputActionReference.action.started += HandleStarted;
            _inputActionReference.action.canceled += HandleCanceled;
        }

        private void OnDisable()
        {
            if (_inputActionReference == null || _inputActionReference.action == null)
                return;

            _inputActionReference.action.started -= HandleStarted;
            _inputActionReference.action.canceled -= HandleCanceled;
            _inputActionReference.action.Disable();
        }

        private void Update()
        {
            if (_isCompleted || !_isHolding) 
                return;

            _currentHoldTime += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(_currentHoldTime / _requiredHoldTime);
            OnProgressChanged?.Invoke(progress);

            if (_currentHoldTime >= _requiredHoldTime)
            {
                _isCompleted = true;
                _isHolding = false;
                OnCompleted?.Invoke();
            }
        }

        private void HandleStarted(InputAction.CallbackContext context)
        {
            if (_isCompleted) 
                return;

            _isHolding = true;
            _currentHoldTime = 0f;
        }

        private void HandleCanceled(InputAction.CallbackContext context)
        {
            if (_isCompleted || !_isHolding) 
                return;

            ResetHold();
        }

        public void ResetHold()
        {
            _isHolding = false;
            _currentHoldTime = 0f;
            OnProgressChanged?.Invoke(0f);
            OnCancelled?.Invoke();
        }
    }
}