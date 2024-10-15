using System;
using System.Collections.Generic;
using Game.Scripts.InventorySystem;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class InventoryDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject inventorySource;
        [SerializeField] private RectTransform slotsContainer;
        [SerializeField] private GameObject slotDisplayPrefab;

        private readonly Dictionary<ISlot, ISlotDisplay> Dict = new Dictionary<ISlot, ISlotDisplay>();
        
        private IInventory _inventory;

        private void OnInventoryUpdated(IInventory inventory, ISlot slot)
        {
            if (!Dict.TryGetValue(slot, out var slotDisplay))
            {
                slotDisplay = Instantiate(slotDisplayPrefab, slotsContainer, false).GetComponent<ISlotDisplay>();
                Dict.Add(slot, slotDisplay);
            }
            
            slotDisplay.Display(slot);
        }
        
        private void Awake()
        {
            inventorySource.TryGetComponent(out _inventory);
        }

        private void OnEnable()
        {
            _inventory.InventoryUpdated += OnInventoryUpdated;
        }

        private void OnDestroy()
        {
            _inventory.InventoryUpdated -= OnInventoryUpdated;
        }

        private void Start()
        {
            foreach (var slot in _inventory.GetSlotsWithItems())
            {
                OnInventoryUpdated(_inventory, slot);
            }
        }
    }
}