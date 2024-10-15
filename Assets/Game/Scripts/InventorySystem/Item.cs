using UnityEngine;

namespace Game.Scripts.InventorySystem
{
    [DisallowMultipleComponent]
    public sealed class Item : MonoBehaviour, IItem
    {
        [field: SerializeField] public ItemConfig Config { get; private set; }
        
        public void SetConfig(ItemConfig config)
        {
            Config = config;
        }
    }
}