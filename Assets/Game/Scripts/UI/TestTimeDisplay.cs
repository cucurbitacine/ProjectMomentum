using System;
using Game.Scripts.Interactions;
using Game.Scripts.Levels;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class TestTimeDisplay : MonoBehaviour
    {
        [SerializeField] private GameObject storage;
        [SerializeField] private LevelController level;
         
        [Header("UI")]
        [SerializeField] private TMP_Text timeText;
        
        private IStorage _storage;

        [SerializeField] private float _timeInSeconds;

        private void Awake()
        {
            storage.TryGetComponent(out _storage);
        }

        private void Start()
        {
            _timeInSeconds = 0f;
        }
        
        private void Update()
        {
            if (_storage.Amount < level.TotalAmount)
            {
                var time = TimeSpan.FromSeconds(_timeInSeconds);
                
                timeText.text = $"{DateTime.FromFileTime(time.Ticks):HH:mm:ss}";
                
                _timeInSeconds += Time.deltaTime;
            }
        }
    }
}
