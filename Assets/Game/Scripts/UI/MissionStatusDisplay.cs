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
        
        private void Update()
        {
            savedText.text = $"{level.SavedAmount:00}/{level.TotalAmount:00} saved";
            
            var time = TimeSpan.FromSeconds(level.TimeSpent);
                
            timeText.text = $"Mission time: {DateTime.FromFileTime(time.Ticks):HH:mm:ss}";
        }
    }
}
