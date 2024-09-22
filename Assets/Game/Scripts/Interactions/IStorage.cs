using UnityEngine;

namespace Game.Scripts.Interactions
{
    public interface IStorage
    {
        public int Amount { get; set; }
        public Vector3 Gateway { get; }
    }
}