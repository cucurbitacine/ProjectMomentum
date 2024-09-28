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
        [field: SerializeField] public LevelState State { get; private set; } = LevelState.Starting;
        
        [Space]
        [SerializeField] [Min(0f)] private float dieDuration = 2f;
        [SerializeField] [Min(0f)] private float saveDuration = 2f;
        [SerializeField] [Min(0f)] private float failDuration = 2f;
        [SerializeField] [Min(0f)] private float completeDuration = 2f;
        [SerializeField] [Min(0f)] private float restartDuration = 2f;
        [SerializeField] [Min(0f)] private float quitDuration = 2f;
        
        [Space]
        [SerializeField] private LazyComponent<PlayerController> lazyPlayer;
        [SerializeField] private StorageBase evacuatedStorage;
        
        [Header("Level Building")]
        [SerializeField] private List<ShipwreckGenerator> shipwreckGenerators = new List<ShipwreckGenerator>();
        [SerializeField] private List<SpaceFieldGenerator> spaceGenerators = new List<SpaceFieldGenerator>();

        [Header("UI")]
        [SerializeField] private TextDisplayBase textDisplay;
        
        private readonly List<List<Shipwreck>> _shipwreckQueue = new List<List<Shipwreck>>();
        private int _currentIndex = -1;
        private float _fuelAmount; 
        private int _healthAmount; 
        
        public int TotalAmount { get; private set; }
        
        public int SavedAmount => evacuatedStorage?.Amount ?? 0;
        public float TimeSpent { get; private set; }
        public int HealthSpent { get; private set; }
        public float FuelSpent { get; private set; }
        
        public PlayerController Player => (lazyPlayer ??= new LazyComponent<PlayerController>(gameObject)).Value;

        public void RestartLevel()
        {
            if (State == LevelState.Loading) return;
            State = LevelState.Loading;
            
            StartCoroutine(Restarting());
        }

        public void ExitLevel()
        {
            if (State == LevelState.Loading) return;
            State = LevelState.Loading;
            
            StartCoroutine(Quiting());
        }
        
        public void Build()
        {
            if (State != LevelState.Starting) return;
            
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
        
        private GameSessionData CreateGameSessionData()
        {
            return new GameSessionData()
            {
                savedAmount = SavedAmount,
                timeSpent = TimeSpent,
                healthSpent = HealthSpent,
                fuelSpent = FuelSpent,
            };
        }

        private void SaveSession()
        {
            var session = CreateGameSessionData();
            
            GameSessionTable.Save(session);
        }
        
        private void HandleFuel(float fuel)
        {
            if (State != LevelState.Playing) return;

            var delta = fuel - _fuelAmount;
            if (delta < 0)
            {
                FuelSpent -= delta;
            }
            _fuelAmount = fuel;
            
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
        
        private void HandleHealth(int health)
        {
            if (State != LevelState.Playing) return;
            
            var delta = health - _healthAmount;
            if (delta < 0)
            {
                HealthSpent -= delta;
            }
            _healthAmount = health;
        }
        
        private void HandlePlayerDeath()
        {
            if (State != LevelState.Playing) return;
            State = LevelState.Failed;

            StartCoroutine(Dying());
        }
        
        private void HandleMissionComplete(int value)
        {
            if (State != LevelState.Playing) return;
            if (SavedAmount < TotalAmount) return;
            State = LevelState.Completed;
            
            StartCoroutine(Saving());
        }
        
        private IEnumerator Dying()
        {
            textDisplay.Display("> Wasted spaceship...", Color.red);

            yield return new WaitForSeconds(dieDuration);

            FailLevel();
        }
        
        private IEnumerator Saving()
        {
            textDisplay.Display("> Saved catstronauts...", Color.green);

            yield return new WaitForSeconds(saveDuration);

            CompleteLevel();
        }
        
        public void FailLevel()
        {
            State = LevelState.Failed;

            StartCoroutine(Failing());
        }
        
        public void CompleteLevel()
        {
            State = LevelState.Completed;

            SaveSession();

            StartCoroutine(Completing());
        }
        
        private IEnumerator Failing()
        {
            textDisplay.Display("> Mission failed...", Color.red);
            
            yield return new WaitForSeconds(failDuration);
            
            RestartLevel();
        }
        
        private IEnumerator Completing()
        {
            textDisplay.Display("> Mission completed...", Color.green);
            
            yield return new WaitForSeconds(completeDuration);
            
            ExitLevel();
        }
        
        private IEnumerator Restarting()
        {
            textDisplay.Display("> Restart mission...", Color.red);
            
            yield return new WaitForSeconds(restartDuration);
            
            SceneManager.LoadScene(1);
        }
        
        private IEnumerator Quiting()
        {
            textDisplay.Display("> Interrupt mission...", Color.red);
            
            yield return new WaitForSeconds(quitDuration);
            
            SceneManager.LoadScene(0);
        }
        
        private void OnEnable()
        {
            Player.Health.OnValueChanged += HandleHealth;
            Player.Health.OnDied += HandlePlayerDeath;
            Player.Spaceship.Fuel.OnValueChanged += HandleFuel;

            evacuatedStorage.OnAmountChanged += HandleMissionComplete;
        }

        private void OnDisable()
        {
            Player.Health.OnValueChanged -= HandleHealth;
            Player.Health.OnDied -= HandlePlayerDeath;
            Player.Spaceship.Fuel.OnValueChanged -= HandleFuel;
            
            evacuatedStorage.OnAmountChanged -= HandleMissionComplete;
        }

        private void Start()
        {
            Build();

            _healthAmount = Player.Health.Value;
            _fuelAmount = Player.Spaceship.Fuel.Value;

            State = LevelState.Playing;
        }

        private void Update()
        {
            if (State == LevelState.Playing && SavedAmount < TotalAmount)
            {
                TimeSpent += Time.deltaTime;
            }
        }

        private void OnDestroy()
        {
            SosShipwrecks(-1);
        }
        
        public enum LevelState
        {
            Starting,
            Playing,
            Failed,
            Completed,
            Loading,
        }
    }
}