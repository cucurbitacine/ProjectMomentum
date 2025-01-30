using System;
using Game.Scripts.Levels;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class GameSessionDataDisplay : MonoBehaviour
    {
        [SerializeField] private GameSessionData data;

        [Space]
        [SerializeField] private TMP_Text savedText;
        [SerializeField] private TMP_Text timeText;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text fuelText;
        
        public void UpdateData()
        {
            savedText.text = $"{data.savedAmount}";
            timeText.text = $"{DateTime.FromFileTime(TimeSpan.FromSeconds(data.timeSpent).Ticks):HH:mm:ss}";
            healthText.text = $"{data.healthSpent}";
            fuelText.text = $"{data.fuelSpent:F1}";
        }
        
        public void SetData(GameSessionData newData)
        {
            data = newData;

            UpdateData();
        }
    }
}
