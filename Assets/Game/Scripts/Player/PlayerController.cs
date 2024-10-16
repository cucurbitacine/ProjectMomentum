using System;
using System.Linq;
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

        private float _inventoryMass;
        private float _fuelMass;
        
        private LazyComponent<Spaceship> _lazySpaceship;
        private LazyComponent<Health> _lazyHealth;
        private LazyComponent<Interactor> _lazyInteractor;
        
        public Spaceship Spaceship => (_lazySpaceship ??= new LazyComponent<Spaceship>(gameObject)).Value;
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        public Interactor Interactor => (_lazyInteractor ??= new LazyComponent<Interactor>(gameObject)).Value;
        
        public Transform Gateway => Spaceship.transform;
        
        #region IInventory
        
        private LazyComponent<IInventory> _lazyInventory;
        private IInventory Inventory => (_lazyInventory ??= new LazyComponent<IInventory>(gameObject)).Value;
        
        public event Action<IInventory, ISlot> InventoryUpdated;
        public int CountSlots => Inventory.CountSlots;
        public ISlot GetSlot(int index)
        {
            return Inventory.GetSlot(index);
        }

        #endregion
        
        private void UpdateMass()
        {
            Spaceship.mass = massShip + _inventoryMass + _fuelMass;
        }

        private void OnInventoryUpdated(IInventory inv, ISlot slt)
        {
            _inventoryMass = GetInventoryMass();
            
            UpdateMass();
            
            InventoryUpdated?.Invoke(this, slt);
        }

        private void HandleFuel(float fuel)
        {
            _fuelMass = GetFuelMass();
            
            UpdateMass();
        }

        private float GetInventoryMass()
        {
            return Inventory.GetSlotsWithItems().Select(slot => (slot, item: slot.GetItem())).Sum(x => x.slot.CountItems * x.item.GetMass());
        }

        private float GetFuelMass()
        {
            return Fuel.MassPerUnit * Spaceship.Fuel.Value;
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
            _inventoryMass = GetInventoryMass();
            _fuelMass = GetFuelMass();
            
            UpdateMass();
        }
    }
}