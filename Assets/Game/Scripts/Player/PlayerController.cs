using Game.Scripts.Combat;
using Game.Scripts.Control;
using Game.Scripts.Core;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(SpaceshipController))]
    [RequireComponent(typeof(Interactor))]
    public class PlayerController : MonoBehaviour, IStorage
    {
        [field: SerializeField] public int Amount { get; set; }
        
        private LazyComponent<SpaceshipController> _lazySpaceship;
        private LazyComponent<Health> _lazyHealth;
        private LazyComponent<Interactor> _lazyInteractor;

        public SpaceshipController Spaceship => (_lazySpaceship ??= new LazyComponent<SpaceshipController>(gameObject)).Value;
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        public Interactor Interactor => (_lazyInteractor ??= new LazyComponent<Interactor>(gameObject)).Value;

        public Vector3 Gateway => Spaceship.position;
    }
}
