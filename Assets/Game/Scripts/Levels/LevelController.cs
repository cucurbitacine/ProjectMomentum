using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class LevelController : MonoBehaviour
    {
        [field: SerializeField] public int TotalAmount { get; private set; }
        
        [Header("Level Building")]
        [SerializeField] private bool buildOnStart = false;
        [SerializeField] private List<ShipwreckGenerator> shipwreckGenerators = new List<ShipwreckGenerator>();
        [SerializeField] private List<AsteroidFieldGenerator> asteroidGenerators = new List<AsteroidFieldGenerator>();

        private readonly List<List<Shipwreck>> _shipwreckQueue = new List<List<Shipwreck>>();
        
        private int _currentIndex = -1;
        
        public void Build()
        {
            TotalAmount = 0;
            
            SosShipwrecks(-1);
            
            _shipwreckQueue.Clear();
            
            foreach (var shipwreckGenerator in shipwreckGenerators)
            {
                var shipObjects = shipwreckGenerator.Generate();

                var shipwrecks = new List<Shipwreck>();
                
                foreach (var shipObject in shipObjects)
                {
                    if (shipObject.TryGetComponent<Shipwreck>(out var shipwreck))
                    {
                        shipwrecks.Add(shipwreck);

                        TotalAmount += shipwreck.Amount;
                    }
                }
                
                _shipwreckQueue.Add(shipwrecks);
            }

            SosShipwrecks(0);
            
            foreach (var asteroidFieldGenerator in asteroidGenerators)
            {
                asteroidFieldGenerator.Generate();
            }
        }
        
        private void SosShipwrecks(int index)
        {
            if (_currentIndex >= 0)
            {
                var previousShips = _shipwreckQueue[_currentIndex];

                foreach (var ship in previousShips)
                {
                    ship.OnCompleted -= HandleShipwreck;
                    
                    if (gameObject.scene.isLoaded)
                    {
                        ship.Sos(false);
                    }
                }
            }

            _currentIndex = index;
            
            if (_currentIndex < 0) return;
            if (_shipwreckQueue.Count <= _currentIndex) return;
            
            var ships = _shipwreckQueue[_currentIndex];

            foreach (var ship in ships)
            {
                ship.OnCompleted += HandleShipwreck;
                ship.Sos(true);
            }
        }

        private void HandleShipwreck()
        {
            if (_currentIndex < 0) return;
            if (_shipwreckQueue.Count <= _currentIndex) return;
            
            if (_shipwreckQueue[_currentIndex].All(s => s.IsCompleted))
            {
                SosShipwrecks(_currentIndex + 1);
            }
        }

        private void Start()
        {
            if (buildOnStart)
            {
                Build();
            }
        }

        private void OnDestroy()
        {
            SosShipwrecks(-1);
        }
    }
}