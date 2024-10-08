using System;
using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Control
{
    public class SpaceshipEffectController : MonoBehaviour
    {
        [SerializeField] private SpaceshipController spaceship;
        
        [Header("Effects")]
        [SerializeField] private GameObject mainEngineEffect;
        [Space]
        [SerializeField] private GameObject frontLeftEngineEffect;
        [SerializeField] private GameObject frontRightEngineEffect;
        [Space]
        [SerializeField] private GameObject backLeftEngineEffect;
        [SerializeField] private GameObject backRightEngineEffect;
        [Space]
        [SerializeField] private GameObject leftFrontEngineEffect;
        [SerializeField] private GameObject leftBackEngineEffect;
        [Space]
        [SerializeField] private GameObject rightFrontEngineEffect;
        [SerializeField] private GameObject rightBackEngineEffect;
        
        private float _rotation;
        private Vector2 _movement;
        
        private const float EffectTolerance = 0.1f;
        
        private void HandleJetEngine(float value)
        {
            mainEngineEffect.PlaySafe(value > 0f);
        }
        
        private void HandleRotation(float value)
        {
            _rotation = value;
        }

        private void HandleMovement(Vector2 value)
        {
            _movement = value;
        }

        private void Awake()
        {
            if (spaceship == null)
            {
                spaceship = GetComponentInParent<SpaceshipController>();
            }
        }

        private void OnEnable()
        {
            spaceship.OnJetChanged += HandleJetEngine;
            spaceship.OnRotationChanged += HandleRotation;
            spaceship.OnMovementChanged += HandleMovement;
        }
        
        private void OnDisable()
        {
            spaceship.OnJetChanged -= HandleJetEngine;
            spaceship.OnRotationChanged -= HandleRotation;
            spaceship.OnMovementChanged -= HandleMovement;
        }
        
        private void Update()
        {
            frontLeftEngineEffect.PlaySafe(_movement.y < -EffectTolerance || _rotation > EffectTolerance);
            frontRightEngineEffect.PlaySafe(_movement.y < -EffectTolerance || _rotation < -EffectTolerance);
            
            backLeftEngineEffect.PlaySafe(_movement.y > EffectTolerance || _rotation < -EffectTolerance);
            backRightEngineEffect.PlaySafe(_movement.y > EffectTolerance || _rotation > EffectTolerance);
            
            leftFrontEngineEffect.PlaySafe(_movement.x > EffectTolerance || _rotation < -EffectTolerance);
            leftBackEngineEffect.PlaySafe(_movement.x > EffectTolerance || _rotation > EffectTolerance);
            
            rightFrontEngineEffect.PlaySafe(_movement.x < -EffectTolerance || _rotation > EffectTolerance);
            rightBackEngineEffect.PlaySafe(_movement.x < -EffectTolerance || _rotation < -EffectTolerance);
        }
    }
}