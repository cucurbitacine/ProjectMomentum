using CucuTools;
using UnityEngine;

namespace Game.Scripts.Combat
{
    public class DamageByCollision : MonoBehaviour
    {
        [SerializeField] private Health health;

        [Header("Settings")]
        [SerializeField] [Min(0f)] private float damageTimeout = 0.1f;
        [SerializeField] [Min(1)] private int damagePerMomentum = 1;
        [SerializeField] private LayerMask layerMask = 1;
        
        [Header("VFX")]
        [SerializeField] private GameObject hitEffectPrefab;

        private float _lastHitTime = float.MinValue;

        private void PlayEffect(Vector2 point, float relativeSpeed)
        {
            if (hitEffectPrefab)
            {
                var hitEffect = SmartPrefab.SmartInstantiate(hitEffectPrefab, point, Quaternion.identity);

                if (hitEffect.TryGetComponent<ParticleSystem>(out var particle))
                {
                    var mainModule = particle.main;
                    var startSpeedParam = mainModule.startSpeed;
                    startSpeedParam.constantMax = relativeSpeed;
                    mainModule.startSpeed = startSpeedParam;
                }
                        
                hitEffect.PlaySafe();
            }
        }
        
        private void Awake()
        {
            if (health == null) health = GetComponentInParent<Health>();
        }
        
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.transform.CompareLayer(layerMask) && other.rigidbody)
            {
                var contact = other.GetContact(0);
                
                var relativeVelocity = other.relativeVelocity;
                var relativeSpeed = relativeVelocity.magnitude;

                var massScale = 1f;// contact.rigidbody.mass > 0f ? contact.otherRigidbody.mass / contact.rigidbody.mass : 1f;
                var normalScale = Mathf.Abs(Vector2.Dot(relativeVelocity, contact.normal));
                
                var momentum = relativeSpeed * massScale * normalScale;

                var damageAmount = (int)(momentum * damagePerMomentum);

                if (damageAmount >= 0)
                {
                    if (damageAmount > 0 && Time.time - _lastHitTime > damageTimeout)
                    {
                        _lastHitTime = Time.time;
                    
                        health.Damage(damageAmount);
                    }
                
                    PlayEffect(contact.point, relativeSpeed);
                }
            }
        }
    }
}