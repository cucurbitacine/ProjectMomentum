using Game.Scripts.Combat;
using Game.Scripts.Control;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpaceshipController))]
    public class PlayerController : MonoBehaviour
    {
        private LazyComponent<SpaceshipController> _lazySpaceship;
        private LazyComponent<Health> _lazyHealth;

        public SpaceshipController Spaceship => (_lazySpaceship ??= new LazyComponent<SpaceshipController>(gameObject)).Value;
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
    }
}
