using Game.Scripts.InventorySystem;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class StorageDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject storage;

        [Header("Settings")]
        [SerializeField] private string prefix = string.Empty;
        [SerializeField] private string suffix = string.Empty;
        
        [Header("UI")]
        [SerializeField] private TMP_Text amountText;

        private IInventory _inventory;

        private void Awake()
        {
            storage.TryGetComponent(out _inventory);
        }

        private void Update()
        {
            amountText.text = $"{prefix}{_inventory.GetItems()}{suffix}";
        }
    }
}