using System;
using Game.Scripts.Interactions;
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

        private IStorage _storage;

        private void Awake()
        {
            storage.TryGetComponent(out _storage);
        }

        private void Update()
        {
            amountText.text = $"{prefix}{_storage.Amount}{suffix}";
        }
    }
}