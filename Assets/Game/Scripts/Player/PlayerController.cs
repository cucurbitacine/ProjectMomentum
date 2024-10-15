using System;
using CucuTools;
using Game.Scripts.Combat;
using Game.Scripts.Control;
using Game.Scripts.Interactions;
using Game.Scripts.InventorySystem;
using UnityEngine;

namespace Game.Scripts.Player
{
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(Spaceship))]
    [RequireComponent(typeof(Interactor))]
    public class PlayerController : MonoBehaviour, IInventoryWithGateway
    {
        public static PlayerController Player { get; private set; }

        [Header("Mass")]
        [SerializeField] [Min(0f)] private float massShip = 50f;
        [SerializeField] [Min(0f)] private float massPerUnit = 5f;
        [SerializeField] [Min(0f)] private float massPerFuelPercent = 0.5f;
        
        private LazyComponent<Spaceship> _lazySpaceship;
        private LazyComponent<Health> _lazyHealth;
        private LazyComponent<Interactor> _lazyInteractor;
        private LazyComponent<IInventory> _lazyInventory;

        public Spaceship Spaceship => (_lazySpaceship ??= new LazyComponent<Spaceship>(gameObject)).Value;
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        public Interactor Interactor => (_lazyInteractor ??= new LazyComponent<Interactor>(gameObject)).Value;
        public IInventory Inventory => (_lazyInventory ??= new LazyComponent<IInventory>(gameObject)).Value;

        public Transform Gateway => Spaceship.transform;
        
        #region IInventory

        public event Action<IInventory, ISlot> InventoryUpdated;
        public int CountSlots => Inventory.CountSlots;
        public ISlot GetSlot(int index)
        {
            return Inventory.GetSlot(index);
        }

        #endregion
        
        private void UpdateMass()
        {
            Spaceship.mass = (massShip + GetStorageMass() + GetFuelMass()) * 0.01f;
        }

        private void OnInventoryUpdated(IInventory inv, ISlot slt)
        {
            UpdateMass();
            
            InventoryUpdated?.Invoke(this, slt);
        }

        private void HandleFuel(float fuel)
        {
            UpdateMass();
        }

        private float GetStorageMass()
        {
            return Inventory.CountItems() * massPerUnit;
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
            Inventory.InventoryUpdated += OnInventoryUpdated;
            Spaceship.Fuel.ValueChanged += HandleFuel;
        }

        private void OnDisable()
        {
            Inventory.InventoryUpdated -= OnInventoryUpdated;
            Spaceship.Fuel.ValueChanged -= HandleFuel;
        }
        
        private void Start()
        {
            UpdateMass();
        }
    }
}