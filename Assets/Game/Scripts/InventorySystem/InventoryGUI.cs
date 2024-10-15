using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.InventorySystem
{
    public class InventoryGUI : MonoBehaviour
    {
        [Serializable]
        public class SlotDisplay
        {
            public string name;
            public int amount;

            public readonly ISlot slot;
            
            public SlotDisplay(ISlot slot)
            {
                this.slot = slot;
            }

            public void OnSlotUpdated()
            {
                name = slot.TryPeek(out var item) ? item.name : "";
                amount = slot.CountItems;
            }
        }

        private readonly HashSet<SlotDisplay> slots = new HashSet<SlotDisplay>();
        
        private IInventory _inventory;

        private void OnInventoryUpdated(IInventory inventory, ISlot slot)
        {
            if (inventory != _inventory) return;

            foreach (var slotDisplay in slots.Where(slotDisplay => slotDisplay.slot == slot))
            {
                slotDisplay.OnSlotUpdated();
            }
        }
        
        private void Awake()
        {
            TryGetComponent(out _inventory);
        }

        private void OnEnable()
        {
            _inventory.InventoryUpdated += OnInventoryUpdated;
        }
        
        private void OnDisable()
        {
            _inventory.InventoryUpdated -= OnInventoryUpdated;
        }

        private void Start()
        {
            for (var i = 0; i < _inventory.CountSlots; i++)
            {
                var slot = _inventory.GetSlot(i);
                var slotDisplay = new SlotDisplay(slot);
                slotDisplay.OnSlotUpdated();
                
                slots.Add(slotDisplay);
            }
        }

        private void OnGUI()
        {
            foreach (var slot in slots)
            {
                GUILayout.Box($"{slot.name}{(slot.amount > 1 ? $" {slot.amount} un." : "")}");
            }
        }
    }
}