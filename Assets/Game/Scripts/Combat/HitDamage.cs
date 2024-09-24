using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Combat
{
    public class HitDamage : MonoBehaviour
    {
        [SerializeField] [Min(1)] private int damagePerMomentum = 1;
        [SerializeField] private ParticleSystem hitEffectPrefab;
        
        [Header("References")]
        [SerializeField] private Health health;

        private void Awake()
        {
            if (health == null) health = GetComponentInParent<Health>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (health.IsDead) return;

            if (other.rigidbody)
            {
                var relativeSpeed = other.relativeVelocity.magnitude;
                var momentum = relativeSpeed * other.rigidbody.mass;

                var damageAmount = (int)(momentum * damagePerMomentum);

                if (damageAmount > 0)
                {
                    health.Damage(damageAmount);

                    if (hitEffectPrefab)
                    {
                        var hitEffect = SmartPrefab.SmartInstantiate(hitEffectPrefab);
                        hitEffect.transform.position = other.GetContact(0).point;
                        
                        var mainModule = hitEffect.main;
                        var startSpeedParam = mainModule.startSpeed;
                        startSpeedParam.constantMax = relativeSpeed;
                        mainModule.startSpeed = startSpeedParam;
                        
                        hitEffect.Play();
                    }
                    
                    //Debug.Log($"Damage: [{damageAmount:F1}] \"{other.rigidbody.name}\" > \"{health.name}\"");
                }
            }
        }
    }
}