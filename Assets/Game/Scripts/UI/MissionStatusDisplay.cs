using System;
using Game.Scripts.Levels;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class MissionStatusDisplay : MonoBehaviour
    {
        [SerializeField] private LevelController level;
         
        [Header("UI")]
        [SerializeField] private TMP_Text savedText;
        [SerializeField] private TMP_Text timeText;
        
        [SerializeField] private float _timeInSeconds;
        
        private void Start()
        {
            _timeInSeconds = 0f;
        }
        
        private void Update()
        {
            savedText.text = $"{level.SavedAmount:00}/{level.TotalAmount:00} saved";
            
            if (level.SavedAmount < level.TotalAmount)
            {
                var time = TimeSpan.FromSeconds(_timeInSeconds);
                
                timeText.text = $"Mission time: {DateTime.FromFileTime(time.Ticks):HH:mm:ss}";
                
                _timeInSeconds += Time.deltaTime;
            }
        }
    }
}
