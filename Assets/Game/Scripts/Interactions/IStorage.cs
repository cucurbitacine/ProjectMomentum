using System;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IStorage
    {
        public int Amount { get; set; }
        
        public event Action<int> OnAmountChanged;
    }

    public interface IGateway
    {
        public Transform Gateway { get; }
    }
    
    public interface IStorageWithGateway : IStorage, IGateway
    {
        
    }
}