using CucuTools;
using UnityEngine;

namespace Game.Scripts.Control
{
    public class SpaceshipEffect : MonoBehaviour
    {
        [SerializeField] private Spaceship spaceship;
        
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
        
        private bool isPaused;
        
        private float _rotation;
        private Vector2 _movement;
        
        private const float EffectTolerance = 0.1f;
        
        private void OnJetChanged(float value)
        {
            if (isPaused) return;
            
            mainEngineEffect.PlaySafe(value > 0f);
        }
        
        private void OnRotationChanged(float value)
        {
            _rotation = value;
        }

        private void OnMovementChanged(Vector2 value)
        {
            _movement = value;
        }
        
        private void OnSpaceshipPaused(bool paused)
        {
            isPaused = paused;
            
            if (!paused) return;
            
            mainEngineEffect.PlaySafe(false);
            
            frontLeftEngineEffect.PlaySafe(false);
            frontRightEngineEffect.PlaySafe(false);
            
            backLeftEngineEffect.PlaySafe(false);
            backRightEngineEffect.PlaySafe(false);
            
            leftFrontEngineEffect.PlaySafe(false);
            leftBackEngineEffect.PlaySafe(false);
            
            rightFrontEngineEffect.PlaySafe(false);
            rightBackEngineEffect.PlaySafe(false);
        }
        
        private void Awake()
        {
            if (spaceship == null)
            {
                spaceship = GetComponentInParent<Spaceship>();
            }
        }

        private void OnEnable()
        {
            spaceship.JetChanged += OnJetChanged;
            spaceship.RotationChanged += OnRotationChanged;
            spaceship.MovementChanged += OnMovementChanged;

            spaceship.Paused += OnSpaceshipPaused;
        }

        private void OnDisable()
        {
            spaceship.JetChanged -= OnJetChanged;
            spaceship.RotationChanged -= OnRotationChanged;
            spaceship.MovementChanged -= OnMovementChanged;
            
            spaceship.Paused -= OnSpaceshipPaused;
        }
        
        private void Update()
        {
            if (isPaused) return;
            
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