using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CucuTools;
using Game.Scripts.InventorySystem;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Levels
{
    public class LevelController : MonoBehaviour
    {
        [field: SerializeField] public LevelState State { get; private set; } = LevelState.Starting;
        
        [Header("Level Settings")]
        [SerializeField] [Min(0f)] private float maxPlayerDistance = 500f;
        
        [Header("Game Entities")]
        [SerializeField] private LazyComponent<PlayerController> lazyPlayer;
        [SerializeField] private InventoryBase evacuatedStorage;

        [Header("Level Building")]
        [SerializeField] private ChunkGenerationSystem generatorSystem;
        [SerializeField] private List<RingSpawnerSystem> shipwrecksSpawners = new List<RingSpawnerSystem>();

        [Header("UI")]
        [SerializeField] private TextDisplayBase textDisplay;
        
        [Space]
        [SerializeField] [Min(0f)] private float dieDuration = 2f;
        [SerializeField] [Min(0f)] private float saveDuration = 2f;
        [SerializeField] [Min(0f)] private float failDuration = 2f;
        [SerializeField] [Min(0f)] private float completeDuration = 2f;
        [SerializeField] [Min(0f)] private float restartDuration = 2f;
        [SerializeField] [Min(0f)] private float quitDuration = 2f;
        
        private readonly List<List<Shipwreck>> _shipwreckStack = new List<List<Shipwreck>>();
        private int _selectedStackShipwrecks = -1;
        private float _playerFuel; 
        private int _playerHealth; 
        
        public int MaxScore { get; private set; }
        
        public int Score => evacuatedStorage?.CountItems() ?? 0;
        public float TimeSpent { get; private set; }
        public int HealthSpent { get; private set; }
        public float FuelSpent { get; private set; }
        
        public PlayerController Player => (lazyPlayer ??= new LazyComponent<PlayerController>(gameObject)).Value;

        public void RestartLevel()
        {
            if (State == LevelState.Loading) return;
            State = LevelState.Loading;

            Time.timeScale = 1f;
            
            StartCoroutine(Restarting());
        }

        public void ExitLevel()
        {
            if (State == LevelState.Loading) return;
            State = LevelState.Loading;
            
            Time.timeScale = 1f;
            
            StartCoroutine(Quiting());
        }
        
        public void BuildLevel()
        {
            if (State != LevelState.Starting) return;

            SpawnShipwrecks();

            GenerateLevel();
        }

        private void SpawnShipwrecks()
        {
            ResetShipwrecks();

            SetupShipwrecks();

            StartRescueShipwrecks(0);
        }

        private void GenerateLevel()
        {
            if (generatorSystem)
            {
                generatorSystem.StartGeneration();
            }
            else
            {
                Debug.LogWarning($"There are no Generation System");
            }
        }

        private void ResetShipwrecks()
        {
            StartRescueShipwrecks(-1);
            
            _shipwreckStack.Clear();
        }

        private void SetupShipwrecks()
        {
            MaxScore = 0;
            
            foreach (var spawner in shipwrecksSpawners)
            {
                var spawnedObjects = spawner.Spawn();

                var shipwrecks = new List<Shipwreck>();
                
                foreach (var spawnedObject in spawnedObjects)
                {
                    if (spawnedObject.TryGetComponent<Shipwreck>(out var shipwreck))
                    {
                        shipwrecks.Add(shipwreck);

                        MaxScore += shipwreck.Amount;
                    }
                }
                
                _shipwreckStack.Add(shipwrecks);
            }
        }
        
        private void StartRescueShipwrecks(int index)
        {
            if (0 <= _selectedStackShipwrecks && _selectedStackShipwrecks < _shipwreckStack.Count)
            {
                var previousShipwrecks = _shipwreckStack[_selectedStackShipwrecks];

                foreach (var shipwreck in previousShipwrecks)
                {
                    shipwreck.Rescued -= OnShipwreckRescued;
                    
                    if (gameObject.scene.isLoaded)
                    {
                        shipwreck.StartRescue(false);
                    }
                }
            }

            _selectedStackShipwrecks = index;
            
            if (_selectedStackShipwrecks < 0) return;
            if (_shipwreckStack.Count <= _selectedStackShipwrecks) return;
            
            var selectedShipwrecks = _shipwreckStack[_selectedStackShipwrecks];

            foreach (var shipwreck in selectedShipwrecks)
            {
                shipwreck.Rescued += OnShipwreckRescued;
                
                shipwreck.StartRescue(true);
            }
        }
        
        private void OnShipwreckRescued()
        {
            if (_selectedStackShipwrecks < 0) return;
            if (_shipwreckStack.Count <= _selectedStackShipwrecks) return;
            
            if (_shipwreckStack[_selectedStackShipwrecks].All(s => s.IsCompleted))
            {
                StartRescueShipwrecks(_selectedStackShipwrecks + 1);
            }
        }
        
        private GameSessionData CreateGameSessionData()
        {
            return new GameSessionData()
            {
                savedAmount = Score,
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
        
        private void OnPlayerFueldChanged(float fuel)
        {
            if (State != LevelState.Playing) return;

            var delta = fuel - _playerFuel;
            if (delta < 0)
            {
                FuelSpent -= delta;
            }
            _playerFuel = fuel;
            
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
        
        private void OnPlayerHealthChanged(int health)
        {
            if (State != LevelState.Playing) return;
            
            var delta = health - _playerHealth;
            if (delta < 0)
            {
                HealthSpent -= delta;
            }
            _playerHealth = health;
        }
        
        private void OnPlayerDied()
        {
            if (State != LevelState.Playing) return;
            State = LevelState.Failed;

            StartCoroutine(Dying());
        }
        
        private void OnEvacuatedStorageAmountChanged(IInventory inv, ISlot slt)
        {
            if (State != LevelState.Playing) return;
            if (Score < MaxScore) return;
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
            Time.timeScale = 1f;
            
            State = LevelState.Failed;

            StartCoroutine(Failing());
        }
        
        public void CompleteLevel()
        {
            Time.timeScale = 1f;
            
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
        
        private void OnLevelPaused(bool paused)
        {
            Time.timeScale = paused ? 0.001f : 1f;
        }
        
        private void OnEnable()
        {
            LevelPause.LevelPaused += OnLevelPaused;
            
            
            Player.Health.ValueChanged += OnPlayerHealthChanged;
            Player.Health.Died += OnPlayerDied;
            Player.Spaceship.Fuel.ValueChanged += OnPlayerFueldChanged;

            evacuatedStorage.InventoryUpdated += OnEvacuatedStorageAmountChanged;
        }

        private void OnDisable()
        {
            LevelPause.Pause(false);
            
            LevelPause.LevelPaused -= OnLevelPaused;
            
            Player.Health.ValueChanged -= OnPlayerHealthChanged;
            Player.Health.Died -= OnPlayerDied;
            Player.Spaceship.Fuel.ValueChanged -= OnPlayerFueldChanged;
            
            evacuatedStorage.InventoryUpdated -= OnEvacuatedStorageAmountChanged;
        }

        private void Start()
        {
            BuildLevel();

            _playerHealth = Player.Health.Value;
            _playerFuel = Player.Spaceship.Fuel.Value;

            State = LevelState.Playing;
        }

        private void Update()
        {
            if (State == LevelState.Playing && Score < MaxScore)
            {
                TimeSpent += Time.deltaTime;

                if (Vector2.Distance(Player.Spaceship.position, evacuatedStorage.transform.position) > maxPlayerDistance)
                {
                    RestartLevel();
                }
            }
        }

        private void OnDestroy()
        {
            ResetShipwrecks();
        }

        private void OnDrawGizmosSelected()
        {
            if (evacuatedStorage)
            {
                Gizmos.DrawWireSphere(evacuatedStorage.transform.position, maxPlayerDistance);
            }
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