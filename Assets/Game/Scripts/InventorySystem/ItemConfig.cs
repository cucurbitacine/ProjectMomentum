using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.InventorySystem
{
    [CreateAssetMenu(menuName = "Game/Inventory/Create Item", fileName = "Item", order = 0)]
    public class ItemConfig : ScriptableObject, IPrefabSource, IIconSource
    {
        [field: SerializeField, Min(1)] public int StackMax { get; private set; } = 1;

        [Space]
        [SerializeField] private GameObject prefab;
        [SerializeField] private Sprite icon;
        
        /*
        public int Id => (_lazyId ??= new LazyValue<int>(GetInstanceID)).Value;
        private LazyValue<int> _lazyId;
        */

        public GameObject GetPrefab()
        {
            if (prefab.TryGetComponent<IItem>(out var item))
            {
                item.SetConfig(this);
            }
            
            return prefab;
        }

        public Sprite GetIcon()
        {
            return icon;
        }
    }
}