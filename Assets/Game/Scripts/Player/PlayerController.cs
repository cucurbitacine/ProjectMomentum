using System;
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
        public static PlayerController Player { get; private set; }

        [SerializeField] [Min(0)] private int amount = 0;

        [Header("Mass")]
        [SerializeField] [Min(0f)] private float massShip = 50f;
        [SerializeField] [Min(0f)] private float massPerUnit = 5f;
        [SerializeField] [Min(0f)] private float massPerFuelPercent = 0.5f;

        public event Action<int> OnAmountChanged;

        public int Amount
        {
            get => amount;
            set
            {
                if (amount == value) return;

                amount = value;

                OnAmountChanged?.Invoke(amount);
            }
        }

        private LazyComponent<SpaceshipController> _lazySpaceship;
        private LazyComponent<Health> _lazyHealth;
        private LazyComponent<Interactor> _lazyInteractor;

        public SpaceshipController Spaceship =>
            (_lazySpaceship ??= new LazyComponent<SpaceshipController>(gameObject)).Value;

        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        public Interactor Interactor => (_lazyInteractor ??= new LazyComponent<Interactor>(gameObject)).Value;

        public Vector3 Gateway => Spaceship.position;

        private void UpdateMass()
        {
            Spaceship.mass = (massShip + GetStorageMass() + GetFuelMass()) * 0.01f;
        }

        private void HandleStorage(int value)
        {
            UpdateMass();
        }

        private void HandleFuel(float fuel)
        {
            UpdateMass();
        }

        private float GetStorageMass()
        {
            return Amount * massPerUnit;
        }

        private float GetFuelMass()
        {
            return 100f * massPerFuelPercent * Spaceship.Fuel / Spaceship.FuelMax;
        }

        private void Awake()
        {
            Player = this;
        }

        private void OnEnable()
        {
            OnAmountChanged += HandleStorage;
            Spaceship.OnFuelChanged += HandleFuel;
        }

        private void OnDisable()
        {
            OnAmountChanged -= HandleStorage;
            Spaceship.OnFuelChanged -= HandleFuel;
        }

        private void Start()
        {
            UpdateMass();
        }

        [SerializeField] private float bombRadiusActivation = 4f;
        [SerializeField] private LayerMask bombLayer = 1;
        
        private void FixedUpdate()
        {
            var overlap = Physics2D.OverlapCircle(Spaceship.position, bombRadiusActivation, bombLayer);
            var bombTransform = overlap
                ? (overlap.attachedRigidbody ? overlap.attachedRigidbody.transform : overlap.transform)
                : null;
            
            if (bombTransform && bombTransform.TryGetComponent<TriggerBomb>(out var bomb))
            {
                bomb.Activate();
            }
        }

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(Spaceship.position, bombRadiusActivation);
            }
        }
    }
}