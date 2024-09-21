using System;
using Game.Scripts.Control;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(SpaceshipController))]
    public class PlayerController : MonoBehaviour
    {
        private LazyComponent<SpaceshipController> _lazySpaceship;

        public SpaceshipController Spaceship => (_lazySpaceship ??= new LazyComponent<SpaceshipController>(gameObject)).Value;
    }
}
