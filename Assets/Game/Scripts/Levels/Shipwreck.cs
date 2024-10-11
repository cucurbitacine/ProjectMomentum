using System;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class Shipwreck : MonoBehaviour
    {
        [SerializeField] private StorageBase storage;
        [SerializeField] private Pointer pointer;

        [Space]
        [SerializeField] private GameObject silhouette;
        
        public int Amount => storage?.Amount ?? 0;
        public bool IsCompleted => Amount == 0;
        
        public event Action Rescued;
        
        public void StartRescue(bool value)
        {
            if (value && Amount == 0) return;
            
            pointer.Show(value);
        }
        
        private void HandleStorage(int value)
        {
            silhouette?.SetActive(value > 0);
            
            if (value == 0)
            {
                StartRescue(false);
                
                Rescued?.Invoke();
            }
        }
        
        private void OnEnable()
        {
            storage.AmountChanged += HandleStorage;
        }

        private void OnDisable()
        {
            storage.AmountChanged -= HandleStorage;
        }

        private void Start()
        {
            HandleStorage(storage.Amount);
        }
    }
}
