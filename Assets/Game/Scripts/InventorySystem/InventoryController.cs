using System;
using UnityEngine;

namespace Game.Scripts.InventorySystem
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField] private int amount = 1;
        [SerializeField] private ItemConfig item;
        
        [Space]
        [SerializeField] private GameObject inventorySource;

        private IInventory _inventory;

        private void OnInventoryUpdated(IInventory inventory, ISlot slot)
        {
            //Debug.Log($"{inventory.GetType().Name} Updated in {slot.GetType().Name}");
        }
        
        private void Awake()
        {
            if (inventorySource == null) inventorySource = gameObject;
            
            inventorySource.TryGetComponent(out _inventory);
        }

        private void OnEnable()
        {
            _inventory.InventoryUpdated += OnInventoryUpdated;
        }
        
        private void OnDisable()
        {
            _inventory.InventoryUpdated -= OnInventoryUpdated;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                var put = _inventory.Put(item, amount);
                
                //Debug.Log($"Put {put} / {amount}");
            }
            
            if (Input.GetKeyDown(KeyCode.Q))
            {
                var picked = _inventory.Pick(item, amount);
                
                //Debug.Log($"Picked {picked} / {amount}");
            }
        }
    }
}