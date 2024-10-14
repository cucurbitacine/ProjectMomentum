using System;
using System.Collections.Generic;
using CucuTools;
using UnityEngine;

namespace Game.Scripts.Levels
{
    [DisallowMultipleComponent]
    public class LevelPause : MonoBehaviour
    {
        #region Static

        public static bool IsPaused { get; private set; }
        
        public static event Action<bool> LevelPaused;

        public static void Pause(bool paused)
        {
            if (IsPaused == paused) return;

            IsPaused = paused;
            
            LevelPaused?.Invoke(paused);
        }

        #endregion
        
        private readonly HashSet<IPausable> _set = new HashSet<IPausable>();

        private void OnLevelPaused(bool paused)
        {
            foreach (var pausable in _set)
            {
                pausable.Pause(paused);
            }
        }
        
        private void Awake()
        {
            foreach (var pausable in GetComponentsInChildren<IPausable>())
            {
                _set.Add(pausable);
            }
        }

        private void OnEnable()
        {
            LevelPaused += OnLevelPaused;
        }
        
        private void OnDisable()
        {
            LevelPaused -= OnLevelPaused;
        }
    }
}