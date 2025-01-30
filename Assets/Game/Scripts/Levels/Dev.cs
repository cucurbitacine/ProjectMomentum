using System;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Levels
{
    public class Dev : MonoBehaviour
    {
        [SerializeField] private bool devMode = false;
        [SerializeField] private LevelController level;
        [SerializeField] private GameSessionTable table;

        private void Start()
        {
            table = GameSessionTable.Load();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.E) && Input.GetKeyDown(KeyCode.V))
            {
                devMode = !devMode;
                
                var player = FindObjectOfType<PlayerController>();

                player.Health.Immortal = devMode;
                player.Spaceship.Fuel.Infinity = devMode;
            }

            if (devMode)
            {
                if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.N))
                {
                    level.CompleteLevel();
                }
                
                if (Input.GetKey(KeyCode.F) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.I) &&Input.GetKeyDown(KeyCode.L))
                {
                    level.FailLevel();
                }

                if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.L))
                {
                    GameSessionTable.Clear();
                }
            }
        }

        private void OnGUI()
        {
            if (!devMode) return;
            
            GUILayout.Box($"DEV MODE ON");

            if (table != null)
            {
                foreach (var session in table.sessions)
                {
                    GUILayout.Box($"{session.savedAmount} {session.timeSpent} {session.healthSpent} {session.fuelSpent}");
                }
            }
        }
    }
}
