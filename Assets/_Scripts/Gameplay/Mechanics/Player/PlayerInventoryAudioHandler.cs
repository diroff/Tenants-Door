using Abstractions.Components.Inventory;
using Data;
using Gameplay.Components.Audio;
using UnityEngine;

namespace Gameplay.Mechanics.Player
{
    [RequireComponent(typeof(AudioPlayer))]
    public class PlayerInventoryAudioHandler : MonoBehaviour
    {
        private InventoryComponent _inventory;
        private AudioPlayer _itemAddedAudio;

        private void Awake()
        {
            _inventory = GetComponentInParent<InventoryComponent>();
            _itemAddedAudio = GetComponent<AudioPlayer>();
        }

        private void OnEnable()
        {
            if (_inventory != null)
                _inventory.OnItemAdded += HandleItemAdded;
        }

        private void OnDisable()
        {
            if (_inventory != null)
                _inventory.OnItemAdded -= HandleItemAdded;
        }

        private void HandleItemAdded(ItemStack stack)
        {
            _itemAddedAudio.PlayOneShot();
        }
    }
}