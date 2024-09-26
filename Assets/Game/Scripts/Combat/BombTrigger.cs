using UnityEngine;

namespace Game.Scripts.Combat
{
    public class BombTrigger : MonoBehaviour
    {
        [SerializeField] private float bombRadiusActivation = 4f;
        [SerializeField] private LayerMask bombLayer = 1;

        private readonly Collider2D[] overlap = new Collider2D[32];
        
        private void FixedUpdate()
        {
            var count = Physics2D.OverlapCircleNonAlloc(transform.position, bombRadiusActivation, overlap, bombLayer);

            for (var i = 0; i < count; i++)
            {
                var cld = overlap[i];
                
                var bombTransform = cld
                    ? (cld.attachedRigidbody ? cld.attachedRigidbody.transform : cld.transform)
                    : null;
                
                if (bombTransform && bombTransform.TryGetComponent<Bomb>(out var bomb))
                {
                    bomb.Activate(gameObject);
                }
            }
        }
        
        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(transform.position, bombRadiusActivation);
            }
        }
    }
}