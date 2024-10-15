using System;
using UnityEngine;

namespace Game.Scripts.InventorySystem
{
    [Serializable]
    public class SlotBase : ISlot
    {
        [field: SerializeField, Min(0)] public int CountItems { get; private set; } = 0;
        [field: SerializeField] private ItemConfig Item { get; set; } = null;
        
        public event Action<ISlot> SlotUpdated;
        
        public ItemConfig GetItem()
        {
            return Item;
        }
        
        public bool Contains(ItemConfig item)
        {
            if (CountItems == 0) return false;

            return GetItem() == item;
        }

        public int Pick(ItemConfig item, int amount = 1)
        {
            if (amount <= 0) return 0;
            
            if (!Contains(item)) return 0;

            amount = Mathf.Min(amount, CountItems);

            CountItems -= amount;

            if (CountItems == 0)
            {
                SetItem(null);
            }
            
            SlotUpdated?.Invoke(this);
            
            return amount;
        }

        public int Put(ItemConfig item, int amount = 1)
        {
            if (amount <= 0) return 0;

            if (CountItems != 0 && !Contains(item)) return 0;
            
            amount = Mathf.Min(amount, item.StackMax - CountItems);

            if (CountItems == 0 && amount > 0)
            {
                SetItem(item);
            }

            CountItems += amount;

            SlotUpdated?.Invoke(this);
            
            return amount;
        }

        private void SetItem(ItemConfig item)
        {
            Item = item;
        }

        public void SilenceCopy(ISlot slot)
        {
            if (slot is SlotBase slotBase && slotBase == this) return;
            
            CountItems = slot.CountItems;
            SetItem(slot.GetItem());
        }
    }
}