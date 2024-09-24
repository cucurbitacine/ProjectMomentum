using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IStorage
    {
        public event Action<int> OnAmountChanged;
        
        public int Amount { get; set; }
        public Vector3 Gateway { get; }
    }
}