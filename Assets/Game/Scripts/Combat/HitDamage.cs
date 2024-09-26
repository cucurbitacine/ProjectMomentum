using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Combat
{
    public class HitDamage : MonoBehaviour
    {
        [SerializeField] private Health health;

        [Header("Settings")]
        [SerializeField] [Min(0f)] private float damageTimeout = 0.1f;
        [SerializeField] [Min(1)] private int damagePerMomentum = 1;
        [SerializeField] private LayerMask layerMask = 1;
        
        [Header("VFX")]
        [SerializeField] private GameObject hitEffectPrefab;

        private float _lastHitTime = float.MinValue;
        
        private void Awake()
        {
            if (health == null) health = GetComponentInParent<Health>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.CompareLayer(layerMask) && other.rigidbody)
            {
                var relativeSpeed = other.relativeVelocity.magnitude;
                //var momentum = relativeSpeed * other.rigidbody.mass;
                var momentum = relativeSpeed;

                var damageAmount = (int)(momentum * damagePerMomentum);

                if (damageAmount > 0 && Time.time - _lastHitTime > damageTimeout)
                {
                    _lastHitTime = Time.time;
                    
                    health.Damage(damageAmount);

                    if (hitEffectPrefab)
                    {
                        var hitEffect = SmartPrefab.SmartInstantiate(hitEffectPrefab, other.GetContact(0).point, Quaternion.identity);

                        if (hitEffect.TryGetComponent<ParticleSystem>(out var particle))
                        {
                            var mainModule = particle.main;
                            var startSpeedParam = mainModule.startSpeed;
                            startSpeedParam.constantMax = relativeSpeed;
                            mainModule.startSpeed = startSpeedParam;
                        }
                        
                        hitEffect.Play();
                    }
                }
            }
        }
    }
}