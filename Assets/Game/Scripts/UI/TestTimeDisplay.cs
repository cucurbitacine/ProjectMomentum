using System;
using Game.Scripts.Interactions;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class TestTimeDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject storage;

        [Header("Settings")]
        [SerializeField] private int totalAmount = 10;
        
        [Header("UI")]
        [SerializeField] private TMP_Text timeText;
        
        private IStorage _storage;

        private DateTime _startTime;

        private void Awake()
        {
            storage.TryGetComponent(out _storage);
        }

        private void Start()
        {
            _startTime = DateTime.Now;
        }
        
        private void Update()
        {
            if (_storage.Amount < totalAmount)
            {
                var time = DateTime.Now - _startTime;

                timeText.text = $"{DateTime.FromFileTime(time.Ticks):HH:mm:ss}";
            }
        }
    }
}
