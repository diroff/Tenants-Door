using Abstractions.Components.Inventory;
using Data;
using Gameplay.Components.Audio;
using Gameplay.Mechanics.Interactables;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Gameplay.Mechanics.Player
{
    public class PlayerFlashlight : MonoBehaviour
    {
        [SerializeField] private Light2D _light;
        [SerializeField] private InventoryComponent _inventory;
        [SerializeField] private ItemDefinition _flashlightData;
        [SerializeField] private AudioPlayer _audioPlayer;

        [Inject] private PowerSwitch _powerSwitch;

        private void OnEnable()
        {
            _powerSwitch.OnStateChanged += OnSwitchStateChanged;
            _inventory.OnItemAdded += OnItemAdded;
            OnSwitchStateChanged(_powerSwitch.IsOn);
        }

        private void OnDisable()
        {
            _powerSwitch.OnStateChanged -= OnSwitchStateChanged;
            _inventory.OnItemAdded -= OnItemAdded;
        }

        private void OnSwitchStateChanged(bool isEnabled)
        {
            if (!_inventory.HasItem(_flashlightData))
            {
                _light.gameObject.SetActive(false);
                return;
            }

            _audioPlayer.PlayOneShot();
            _light.gameObject.SetActive(!isEnabled);
        }

        private void OnItemAdded(ItemStack stack)
        {
            if (stack.Item != _flashlightData)
                return;

            OnSwitchStateChanged(_powerSwitch.IsOn);
        }
    }
}
