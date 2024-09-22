using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public class Destroyable : MonoBehaviour
    {
        private LazyComponent<Health> _lazyHealth;
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;

        private void HandleDeath()
        {
            SmartObject.SmartDestroy(gameObject);
        }
        
        private void OnEnable()
        {
            Health.OnDied += HandleDeath;
        }
        
        private void OnDisable()
        {
            Health.OnDied -= HandleDeath;
        }
    }
}
