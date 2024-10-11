using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Combat
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Health))]
    public class Destroyable : MonoBehaviour
    {
        [SerializeField] private bool disableInstead = false;
        
        [Space]
        [SerializeField] private GameObject destroyEffectPrefab;
        
        private LazyComponent<Health> _lazyHealth;
        
        public Health Health => (_lazyHealth ??= new LazyComponent<Health>(gameObject)).Value;
        
        [ContextMenu(nameof(HandleDeath))]
        private void HandleDeath()
        {
            if (disableInstead)
            {
                gameObject.SetActive(false);
            }
            else
            {
                SmartPrefab.SmartDestroy(gameObject);
            }
            
            if (destroyEffectPrefab)
            {
                var destroyEffect = SmartPrefab.SmartInstantiate(destroyEffectPrefab, transform.position, transform.rotation);
                destroyEffect.PlaySafe();
            }
        }
        
        private void OnEnable()
        {
            Health.Died += HandleDeath;
        }
        
        private void OnDisable()
        {
            Health.Died -= HandleDeath;
        }
    }
}
