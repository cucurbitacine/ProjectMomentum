using System;
using Game.Scripts.Combat;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SpaceshipHealth : MonoBehaviour
    {
        [SerializeField] private Health health;

        [Header("UI")]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Slider healthSlider;
        
        private void HandleHealth(int value)
        {
            healthSlider.value = (float)health.Value / health.Max;

            healthText.text = $"{(healthSlider.value * 100):F1}%";
        }
        
        private void OnEnable()
        {
            health.OnValueChanged += HandleHealth;
        }
        
        private void OnDisable()
        {
            health.OnValueChanged -= HandleHealth;
        }

        private void Start()
        {
            HandleHealth(0);
        }
    }
}
