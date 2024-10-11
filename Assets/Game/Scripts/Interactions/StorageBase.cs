using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class StorageBase : MonoBehaviour, IStorage
    {
        [SerializeField] private int amount = 0;
        
        public int Amount
        {
            get => amount;
            set
            {
                if (amount == value) return;

                amount = value;
                
                AmountChanged?.Invoke(amount);
            }
        }

        public event Action<int> AmountChanged;
    }
}