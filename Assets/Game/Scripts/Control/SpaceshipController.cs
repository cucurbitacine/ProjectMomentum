using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Control
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceshipController : MonoBehaviour
    {
        [SerializeField] private float powerJetEngine = 1f;
        [SerializeField] private float powerMovement = 1f;
        [SerializeField] private float powerRotation = 1f;

        [field: Space]
        [field: SerializeField] public bool HoldPosition { get; set; } = false;
        [field: SerializeField] public bool HoldRotation { get; set; } = false;
        
        private Vector2 localMovement = Vector2.zero;
        private float rotation = 0f;
        private float jet = 0f;
        
        private Vector2 localForce;
        private float torque;
        
        private LazyComponent<Rigidbody2D> _lazyRigidbody2D;
        
        private Rigidbody2D rigid2d => (_lazyRigidbody2D ??= new LazyComponent<Rigidbody2D>(gameObject)).Value;

        public Vector2 velocity => rigid2d.velocity;
        public float angularVelocity => rigid2d.angularVelocity;
        
        public event Action<Vector2> OnMovementChanged; 
        public event Action<float> OnRotationChanged; 
        public event Action<float> OnJetChanged; 
        
        public void Move(Vector2 value)
        {
            localMovement = value;
        }

        public void Rotate(float value)
        {
            rotation = value;
        }

        public void Jet(float value)
        {
            jet = value;
        }
        
        private void Update()
        {
            //localForce = HoldPosition ? rigid2d.transform.InverseTransformVector(-rigid2d.velocity) : localMovement;
            localForce = HoldPosition ? Vector2.ClampMagnitude(rigid2d.transform.InverseTransformVector(-rigid2d.velocity), 1f) : localMovement;
            torque = HoldRotation ? -rigid2d.angularVelocity * Time.fixedDeltaTime : rotation;
            
            OnMovementChanged?.Invoke(localForce);
            OnRotationChanged?.Invoke(torque);
            OnJetChanged?.Invoke(jet);
        }

        private void FixedUpdate()
        {
            rigid2d.AddForce(rigid2d.transform.TransformVector(localForce) * powerMovement);
            rigid2d.AddTorque(torque * powerRotation);
            
            if (jet > 0f)
            {
                rigid2d.AddForce(rigid2d.transform.TransformVector(Vector2.up) * powerJetEngine);
            }
        }
    }
}
