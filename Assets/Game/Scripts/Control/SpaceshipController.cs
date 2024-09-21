using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Control
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class SpaceshipController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float powerJetEngine = 1f;
        [SerializeField] private float powerMovement = 1f;
        [SerializeField] private float powerRotation = 1f;

        [field: Space]
        [field: SerializeField] public bool StabilizationPosition { get; set; } = false;
        [field: SerializeField] public bool StabilizationRotation { get; set; } = false;
        
        public bool HoldPosition { get; private set; } = false;
        public bool HoldRotation { get; private set; } = false;
        
        private Vector2 _localMovement = Vector2.zero;
        private float _rotation = 0f;
        private float _jet = 0f;
        
        private Vector2 _localForce;
        private float _torque;
        
        private LazyComponent<Rigidbody2D> _lazyRigidbody2D;
        
        private Rigidbody2D rigid2d => (_lazyRigidbody2D ??= new LazyComponent<Rigidbody2D>(gameObject)).Value;

        public Vector2 position => transform.position;
        public Vector2 velocity => rigid2d.velocity;
        public float angularVelocity => rigid2d.angularVelocity;
        
        public event Action<Vector2> OnMovementChanged; 
        public event Action<float> OnRotationChanged; 
        public event Action<float> OnJetChanged; 
        
        public void Move(Vector2 value)
        {
            _localMovement = value;
        }

        public void Rotate(float value)
        {
            _rotation = value;
        }

        public void Jet(float value)
        {
            _jet = value;
        }
        
        private void Update()
        {
            HoldPosition = StabilizationPosition && Mathf.Approximately(_localMovement.sqrMagnitude, 0f) && Mathf.Approximately(_jet, 0f);
            HoldRotation = StabilizationRotation && Mathf.Approximately(_rotation, 0f);
            
            _localForce = HoldPosition ? Vector2.ClampMagnitude(rigid2d.transform.InverseTransformVector(-rigid2d.velocity), 1f) : _localMovement;
            _torque = HoldRotation ? -rigid2d.angularVelocity * Time.fixedDeltaTime : _rotation;
            
            OnMovementChanged?.Invoke(_localForce);
            OnRotationChanged?.Invoke(_torque);
            OnJetChanged?.Invoke(_jet);
        }

        private void FixedUpdate()
        {
            rigid2d.AddForce(rigid2d.transform.TransformVector(_localForce) * powerMovement);
            rigid2d.AddTorque(_torque * powerRotation);
            
            if (_jet > 0f)
            {
                rigid2d.AddForce(rigid2d.transform.TransformVector(Vector2.up) * powerJetEngine);
            }
        }
    }
}
