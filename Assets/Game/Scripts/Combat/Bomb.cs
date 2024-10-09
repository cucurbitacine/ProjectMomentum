using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Combat
{
    public class Bomb : MonoBehaviour
    {
        [SerializeField] private bool activated = false;
        
        [Space]
        [SerializeField] private float explosionRadius = 5f;
        [SerializeField] private float explosionPower = 100f;
        [SerializeField] private int explosionDamage = 20;
        [SerializeField] private float boomDelay = 5f;

        [Space]
        [SerializeField] private GameObject explosionArea;
        [SerializeField] private GameObject boomEffectPrefab;

        [Space]
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private List<AudioClip> activationSfx = new List<AudioClip>();
        
        public void Activate(GameObject actor)
        {
            if (activated) return;
            
            audioSource?.PlayOneShot(activationSfx);
            
            activated = true;
            explosionArea?.SetActive(true);
            Invoke(nameof(Boom), boomDelay);
        }

        [ContextMenu(nameof(Boom))]
        public void Boom()
        {
            if (boomEffectPrefab)
            {
                var boomEffect = SmartPrefab.SmartInstantiate(boomEffectPrefab, transform.position, Quaternion.Euler(0f, 0f, Random.value * 360f));
                boomEffect.PlaySafe();
            }
            
            SmartPrefab.SmartDestroy(gameObject);

            var explosionCenter = (Vector2)transform.position;
            var clds = Physics2D.OverlapCircleAll(explosionCenter, explosionRadius);
            var rigids = clds.ToList().Select(c => c.attachedRigidbody).Where(r => r).Distinct();
            foreach (var rigid in rigids)
            {
                var distance = Vector2.Distance(rigid.position, explosionCenter);
                var direction = (rigid.position - explosionCenter).normalized;
                var scale = (1f - Mathf.Clamp01(distance / explosionRadius));
                
                var power = explosionPower * scale;
                rigid.AddForce(direction * power, ForceMode2D.Impulse);

                if (rigid.TryGetComponent<Health>(out var health))
                {
                    var damageAmount = (int)(explosionDamage * scale);
                    health.Damage(damageAmount);
                }

                if (rigid.TryGetComponent<Bomb>(out var bomb) && bomb != this)
                {
                    bomb.Activate(gameObject);
                }
            }
            
            activated = false;
        }

        private void OnEnable()
        {
            explosionArea?.SetActive(false);
        }

        private void OnValidate()
        {
            if (explosionArea)
            {
                explosionArea.transform.localScale = Vector3.one * explosionRadius * 2f;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}
