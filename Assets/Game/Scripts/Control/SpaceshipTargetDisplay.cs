using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Control
{
    public class SpaceshipTargetDisplay : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private SpaceshipController spaceshipController;
        [Space] [SerializeField] private Image arrowImage;

        private void Awake()
        {
            if (target == null) target = transform;
        }
    }
}