using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core;
using Game.Scripts.Interactions;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Levels
{
    public class LevelController : MonoBehaviour
    {
        [field: SerializeField] public bool Busy { get; private set; }
        
        [Space]
        [SerializeField] private LazyComponent<PlayerController> lazyPlayer;
        [SerializeField] private StorageBase evacuatedStorage;
        
        [Header("Level Building")]
        [SerializeField] private bool buildOnStart = false;
        [SerializeField] private List<ShipwreckGenerator> shipwreckGenerators = new List<ShipwreckGenerator>();
        [SerializeField] private List<SpaceFieldGenerator> spaceGenerators = new List<SpaceFieldGenerator>();

        [Header("UI")]
        [SerializeField] private TextDisplayBase textDisplay;
        
        private readonly List<List<Shipwreck>> _shipwreckQueue = new List<List<Shipwreck>>();
        private int _currentIndex = -1;
        
        public int SavedAmount => evacuatedStorage?.Amount ?? 0;
        public int TotalAmount { get; private set; }
        
        public PlayerController Player => (lazyPlayer ??= new LazyComponent<PlayerController>(gameObject)).Value;
        
        public void RestartLevel()
        {
            if (Busy) return;
            Busy = true;
            
            StartCoroutine(Restarting());
        }

        public void ExitLevel()
        {
            if (Busy) return;
            Busy = true;
            
            StartCoroutine(Restarting());
        }
        
        public void Build()
        {
            TotalAmount = 0;
            
            SosShipwrecks(-1);
            
            _shipwreckQueue.Clear();
            
            foreach (var generator in shipwreckGenerators)
            {
                var shipObjects = generator.Generate();

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
            
            foreach (var generator in spaceGenerators)
            {
                generator.Generate();
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

        private void HandleFuel(float fuel)
        {
            if (Player.Health.IsDead || Busy) return;

            if (fuel <= 0f)
            {
                textDisplay.SetText("> NO FUEL...");
                textDisplay.Display(true);
            }
            else
            {
                textDisplay.Display(false);
            }
        }
        
        private void HandlePlayerDeath()
        {
            if (Busy) return;
            
            textDisplay.SetText("> MISSION FAILED...");
            textDisplay.Display(true);

            StartCoroutine(AfterPlayerDeath());
        }

        private IEnumerator AfterPlayerDeath()
        {
            yield return new WaitForSeconds(3f);
            
            RestartLevel();
        }
        
        private IEnumerator Restarting()
        {
            textDisplay.SetText("> Restart mission...");
            textDisplay.Display(true);
            
            yield return new WaitForSeconds(2f);
            
            SceneManager.LoadScene(0);
        }
        
        private void OnEnable()
        {
            Player.Health.OnDied += HandlePlayerDeath;
            Player.Spaceship.OnFuelChanged += HandleFuel;
        }
        
        private void OnDisable()
        {
            Player.Health.OnDied -= HandlePlayerDeath;
            Player.Spaceship.OnFuelChanged -= HandleFuel;
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