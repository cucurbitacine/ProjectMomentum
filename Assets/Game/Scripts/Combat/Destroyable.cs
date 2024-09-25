using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public class Destroyable : MonoBehaviour
    {
        [SerializeField] private GameObject destroyEffectPrefab;
        
        private LazyComponent<Health> _lazyHealth;
        
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        
        [ContextMenu(nameof(HandleDeath))]
        private void HandleDeath()
        {
            SmartPrefab.SmartDestroy(gameObject);

            if (destroyEffectPrefab)
            {
                var destroyEffect = SmartPrefab.SmartInstantiate(destroyEffectPrefab);
                destroyEffect.transform.position = transform.position;
                destroyEffect.transform.rotation = transform.rotation;
                destroyEffect.PlayFX();
            }
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
