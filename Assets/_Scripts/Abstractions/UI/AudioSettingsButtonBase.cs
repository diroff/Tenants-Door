using Abstractions.UI;
using Reflex.Attributes;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Settings
{
    public abstract class AudioSettingsButtonBase : UIValueButtonBase
    {
        [SerializeField, Range(0.01f, 1f)] private float _step = 0.05f;
        [SerializeField] private Slider _valueSlider;
        [SerializeField] private TMP_Text _valueLabel;

        [Inject] private AudioSettingsService _audioSettingsService;

        protected AudioSettingsService AudioSettingsService => _audioSettingsService;

        private bool _isInitialized;

        protected virtual void Start()
        {
            if (_audioSettingsService == null)
                return;

            if (_valueSlider != null)
                _valueSlider.interactable = false;

            _isInitialized = true;
            RefreshValueView(Mathf.Clamp01(GetCurrentValue()));
        }

        protected sealed override bool OnValueStep(int direction)
        {
            if (!_isInitialized)
                return false;

            float currentValue = Mathf.Clamp01(GetCurrentValue());
            float newValue = Mathf.Clamp01(currentValue + direction * _step);

            if (Mathf.Approximately(currentValue, newValue))
                return false;

            ApplyValue(newValue);
            RefreshValueView(newValue);
            return true;
        }

        protected abstract float GetCurrentValue();

        protected abstract void ApplyValue(float value);

        private void RefreshValueView(float value)
        {
            if (_valueSlider != null)
            {
                float sliderValue = Mathf.Lerp(_valueSlider.minValue,_valueSlider.maxValue,value);
                _valueSlider.SetValueWithoutNotify(sliderValue);
            }

            if (_valueLabel != null)
                _valueLabel.text = $"{Mathf.RoundToInt(value * 100f)}%";
        }
    }
}