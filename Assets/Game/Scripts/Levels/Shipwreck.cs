using System;
using Game.Scripts.InventorySystem;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class Shipwreck : MonoBehaviour
    {
        [SerializeField] private InventoryBase storage;
        [SerializeField] private Pointer pointer;

        [Space]
        [SerializeField] private GameObject silhouette;
        
        public int Amount => storage?.CountItems() ?? 0;
        public bool IsCompleted => Amount == 0;
        
        public event Action Rescued;
        
        public void StartRescue(bool value)
        {
            if (value && Amount == 0) return;
            
            pointer.Show(value);
        }
        
        private void OnInventoryUpdated(IInventory inv, ISlot slt)
        {
            var value = inv.CountItems();
            
            silhouette?.SetActive(value > 0);
            
            if (value == 0)
            {
                StartRescue(false);
                
                Rescued?.Invoke();
            }
        }
        
        private void OnEnable()
        {
            storage.InventoryUpdated += OnInventoryUpdated;
        }

        private void OnDisable()
        {
            storage.InventoryUpdated -= OnInventoryUpdated;
        }

        private void Start()
        {
            OnInventoryUpdated(storage, null);
        }
    }
}
