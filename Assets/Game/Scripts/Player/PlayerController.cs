using System;
using CucuTools;
using Game.Scripts.Combat;
using Game.Scripts.Control;
using Game.Scripts.Interactions;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Spaceship))]
    [RequireComponent(typeof(Interactor))]
    public class PlayerController : MonoBehaviour, IStorageWithGateway
    {
        public static PlayerController Player { get; private set; }

        [SerializeField] [Min(0)] private int amount = 0;

        [Header("Mass")]
        [SerializeField] [Min(0f)] private float massShip = 50f;
        [SerializeField] [Min(0f)] private float massPerUnit = 5f;
        [SerializeField] [Min(0f)] private float massPerFuelPercent = 0.5f;

        public event Action<int> AmountChanged;

        public int Amount
        {
            get => amount;
            set
            {
                if (amount == value) return;

                amount = value;

                AmountChanged?.Invoke(amount);
            }
        }

        private LazyComponent<Spaceship> _lazySpaceship;
        private LazyComponent<Health> _lazyHealth;
        private LazyComponent<Interactor> _lazyInteractor;

        public Spaceship Spaceship => (_lazySpaceship ??= new LazyComponent<Spaceship>(gameObject)).Value;
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        public Interactor Interactor => (_lazyInteractor ??= new LazyComponent<Interactor>(gameObject)).Value;

        public Transform Gateway => Spaceship.transform;
        
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
            return 100f * massPerFuelPercent * Spaceship.Fuel.Value / Spaceship.Fuel.Max;
        }
        
        private void Awake()
        {
            Player = this;
        }

        private void OnEnable()
        {
            AmountChanged += HandleStorage;
            Spaceship.Fuel.ValueChanged += HandleFuel;
        }

        private void OnDisable()
        {
            AmountChanged -= HandleStorage;
            Spaceship.Fuel.ValueChanged -= HandleFuel;
        }
        
        private void Start()
        {
            UpdateMass();
        }
    }
}