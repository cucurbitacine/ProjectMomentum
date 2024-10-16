using System.Linq;
using CucuTools;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.InventorySystem
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MassByInventory : MonoBehaviour, IMass
    {
        [SerializeField] [Min(0f)] private float baseMass = 10f;
        [SerializeField] [Min(0f)] private float inventoryMass = 0f;
        
        private LazyComponent<Rigidbody2D> _lazyRigidbody;
        private LazyComponent<IInventory> _lazyInventory;
        
        public Rigidbody2D Rigid2d => (_lazyRigidbody ??= new LazyComponent<Rigidbody2D>(gameObject)).Value;
        public IInventory Inventory => (_lazyInventory ??= new LazyComponent<IInventory>(gameObject)).Value;

        public float GetMass()
        {
            return baseMass + inventoryMass;
        }

        private void UpdateMass()
        {
            Rigid2d.useAutoMass = false;
            Rigid2d.mass = GetMass() * IMass.Mass2Rigid;
        }

        private float GetInventoryMass(IInventory inventory)
        {
            return inventory
                .GetSlotsWithItems()
                .Select(slot => (slot, item: slot.GetItem()))
                .Sum(x => x.slot.CountItems * x.item.GetMass());
        }
        
        private void OnInventoryUpdated(IInventory inventory, ISlot slot)
        {
            inventoryMass = GetInventoryMass(inventory);
            
            UpdateMass();
        }
        
        private void OnEnable()
        {
            Inventory.InventoryUpdated += OnInventoryUpdated;
            
            OnInventoryUpdated(Inventory, null);
        }

        private void OnDisable()
        {
            Inventory.InventoryUpdated -= OnInventoryUpdated;
        }
    }
}
