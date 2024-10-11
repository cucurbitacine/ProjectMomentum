using System;
using Game.Scripts.Combat;
using Game.Scripts.Control;
using Game.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    public class PlayerStatusDisplay : MonoBehaviour
    {
        [SerializeField] private PlayerController player;

        [Header("UI")]
        [SerializeField] private TMP_Text storageText;
        [Space]
        [SerializeField] private TMP_Text massText;
        [Space]
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Slider healthSlider;
        [Space]
        [SerializeField] private TMP_Text fuelText;
        [SerializeField] private Slider fuelSlider;
        
        private SpaceshipController Spaceship => player.Spaceship;
        private Health Health => player.Health;
            
        private void HandleHealth(int value)
        {
            healthSlider.value = (float)Health.Value / Health.Max;
            healthText.text = $"{(healthSlider.value * 100):F1}%";
        }
        
        private void HandleFuel(float value)
        {
            fuelSlider.value = Spaceship.Fuel.Value / Spaceship.Fuel.Max;
            fuelText.text = $"{(fuelSlider.value * 100):F1}%";
        }
        
        private void HandleStorage(int value)
        {
            storageText.text = $"{value} On Board";
        }
        
        private void OnEnable()
        {
            Health.ValueChanged += HandleHealth;
            player.AmountChanged += HandleStorage;
            Spaceship.Fuel.ValueChanged += HandleFuel;
        }
        
        private void OnDisable()
        {
            Health.ValueChanged -= HandleHealth;
            player.AmountChanged -= HandleStorage;
            Spaceship.Fuel.ValueChanged -= HandleFuel;
        }

        private void Start()
        {
            HandleHealth(Health.Value);
            HandleStorage(player.Amount);
            HandleFuel(Spaceship.Fuel.Value);
        }
        
        private void Update()
        {
            massText.text = $"{(Spaceship.mass * 100):F1} kg";
        }
    }
}