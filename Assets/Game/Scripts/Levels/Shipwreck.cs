using System;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class Shipwreck : MonoBehaviour
    {
        [SerializeField] private CatsStorage storage;
        [SerializeField] private Pointer pointer;

        public int Amount => storage?.Amount ?? 0;
        public bool IsCompleted => Amount == 0;
        
        public event Action OnCompleted;
        
        public void Sos(bool value)
        {
            pointer.Show(value);
        }
        
        private void HandleStorage(int value)
        {
            if (value == 0)
            {
                Sos(false);
                
                OnCompleted?.Invoke();
            }
        }
        
        private void OnEnable()
        {
            storage.OnAmountChanged += HandleStorage;
        }

        private void OnDisable()
        {
            storage.OnAmountChanged -= HandleStorage;
        }
    }
}
