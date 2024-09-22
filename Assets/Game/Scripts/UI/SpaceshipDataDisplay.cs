using System;
using Game.Scripts.Control;
using TMPro;
using UnityEngine;

namespace Game.Scripts.UI
{
    public class SpaceshipDataDisplay : MonoBehaviour
    {
        [SerializeField] private SpaceshipController spaceship;

        [Space]
        [SerializeField] private TMP_Text speedText;
        [SerializeField] private TMP_Text angularSpeedText;
            
        private void Update()
        {
            speedText.text = $"{spaceship.velocity.magnitude:F1} unit/s{(spaceship.StabilizationPosition ? " Auto" : "")}";
            angularSpeedText.text = $"{spaceship.angularVelocity:F1} deg/s{(spaceship.StabilizationRotation ? " Auto" : "")}";
        }
    }
}