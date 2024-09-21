using System;
using Game.Scripts.Combat;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class SpaceshipHealth : MonoBehaviour
    {
        [SerializeField] private Health health;

        [Header("UI")]
        [SerializeField] private Slider slider;
        
        private void HandleHealth(int value)
        {
            slider.value = (float)health.Value / health.Max;
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
