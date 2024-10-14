using System;
using CucuTools;
using UnityEngine;

namespace Game.Scripts.Control
{
    [RequireComponent(typeof(Fuel))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Spaceship : MonoBehaviour, IPausable
    {
        [Header("Settings")]
        [SerializeField] private float powerJetEngine = 1f;
        [SerializeField] private float powerMovement = 1f;
        [SerializeField] private float powerRotation = 1f;

        [field: Space]
        [field: SerializeField] public bool KeepPosition { get; set; } = false;
        [field: SerializeField] public bool KeepRotation { get; set; } = false;
        
        private bool HoldPosition { get; set; } = false;
        private bool HoldRotation { get; set; } = false;
        
        private Vector2 _localMovement = Vector2.zero;
        private float _rotation = 0f;
        private float _jet = 0f;
        
        private Vector2 _localForce;
        private float _torque;
        
        private LazyComponent<Rigidbody2D> _lazyRigidbody2D;
        private LazyComponent<Fuel> _lazyFuel;
        
        private Rigidbody2D rigid2d => (_lazyRigidbody2D ??= new LazyComponent<Rigidbody2D>(gameObject)).Value;
        public Fuel Fuel => (_lazyFuel ??= new LazyComponent<Fuel>(gameObject)).Value;
        
        public Vector2 position => transform.position;
        public Vector2 velocity => rigid2d.velocity;
        public float angularVelocity => rigid2d.angularVelocity;
        public float mass
        {
            get => rigid2d.mass;
            set => rigid2d.mass = value;
        }
        
        public bool IsPaused { get; private set; }
        
        public event Action<bool> Paused;

        public event Action<Vector2> MovementChanged; 
        public event Action<float> RotationChanged; 
        public event Action<float> JetChanged;

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
        
        public void Pause(bool value)
        {
            if (IsPaused == value) return;

            IsPaused = value;
            
            Paused?.Invoke(value);
        }
        
        private void Update()
        {
            if (IsPaused) return;
            
            HoldPosition = KeepPosition && Mathf.Approximately(_localMovement.sqrMagnitude, 0f) && Mathf.Approximately(_jet, 0f);
            HoldRotation = KeepRotation && Mathf.Approximately(_rotation, 0f);
            
            _localForce = HoldPosition ? Vector2.ClampMagnitude(rigid2d.transform.InverseTransformVector(-rigid2d.velocity), 1f) : _localMovement;
            _torque = HoldRotation ? -rigid2d.angularVelocity * Time.fixedDeltaTime : _rotation;
            
            var deltaFuel = Time.deltaTime * (_localForce.magnitude * powerMovement +
                                           Mathf.Abs(_torque) * powerRotation +
                                           _jet * powerJetEngine);
            
            Fuel.Decrease(deltaFuel);

            if (Fuel.Value <= 0f)
            {
                _localForce = Vector2.zero;
                _torque = 0f;
                _jet = 0f;
            }
            
            MovementChanged?.Invoke(_localForce);
            RotationChanged?.Invoke(_torque);
            JetChanged?.Invoke(_jet);
        }

        private void FixedUpdate()
        {
            if (IsPaused) return;
            
            rigid2d.AddForce(rigid2d.transform.TransformVector(_localForce) * powerMovement);
            rigid2d.AddTorque(_torque * powerRotation);
            
            if (_jet > 0f)
            {
                rigid2d.AddForce(rigid2d.transform.TransformVector(Vector2.up) * powerJetEngine);
            }
        }
    }
}
