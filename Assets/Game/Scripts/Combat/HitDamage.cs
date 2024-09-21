using System;
using UnityEngine;

namespace Game.Scripts.Combat
{
    public class HitDamage : MonoBehaviour
    {
        [SerializeField] [Min(1)] private int damagePerMomentum = 1;
        
        [Header("References")]
        [SerializeField] private Health health;

        private void Awake()
        {
            if (health == null) health = GetComponent<Health>();
            if (health == null) health = GetComponentInParent<Health>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (health.IsDead) return;

            if (other.rigidbody)
            {
                var momentum = other.relativeVelocity.magnitude * other.rigidbody.mass;

                var damageAmount = momentum * damagePerMomentum;

                if (damageAmount > 0)
                {
                    health.Damage((int)damageAmount);
                
                    Debug.Log($"Damage: [{damageAmount:F1}] \"{other.rigidbody.name}\" > \"{health.name}\"");
                }
            }
        }
    }
}