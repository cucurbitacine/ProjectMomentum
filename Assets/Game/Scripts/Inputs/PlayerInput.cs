using Game.Scripts.Core;
using Game.Scripts.Levels;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Inputs
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float zoomRate = 1f;

        private Vector2 _inputMove = Vector2.zero;
        private float _inputRotate = 0f;
        private float _inputJet = 0f;
        private bool _inputStabilizePosition = false;
        private bool _inputStabilizeRotation = false;
        private bool _inputNextCamera = false;
        private float _inputZoom = 0f;
        
        private float _orthographicSize = 6f;
        
        private LazyComponent<PlayerController> _lazyPlayer;

        public PlayerController Player => (_lazyPlayer ??= new LazyComponent<PlayerController>(gameObject)).Value;
        
        private void HandleInput(float deltaTime)
        {
            _inputMove.x = Input.GetAxisRaw("Horizontal");
            _inputMove.y = Input.GetAxisRaw("Vertical");
            
            _inputRotate = 0f;
            _inputRotate += Input.GetKey(KeyCode.E) ? -1f : 0f;
            _inputRotate += Input.GetKey(KeyCode.Q) ? 1f : 0f;
            
            _inputJet = Input.GetAxisRaw("Jump");
            
            _inputStabilizeRotation = Input.GetKeyDown(KeyCode.R);
            _inputStabilizePosition = Input.GetKeyDown(KeyCode.T);
            
            _inputNextCamera = Input.GetKeyDown(KeyCode.V);

            _inputZoom = Input.mouseScrollDelta.y +
                         10 * deltaTime * ((Input.GetKey(KeyCode.Z) ? 1f : 0f) + (Input.GetKey(KeyCode.X) ? -1f : 0f));
        }
        
        private void HandlePlayer()
        {
            if (_inputStabilizePosition)
            {
                Player.Spaceship.StabilizationPosition = !Player.Spaceship.StabilizationPosition;
            }
            
            if (_inputStabilizeRotation)
            {
                Player.Spaceship.StabilizationRotation = !Player.Spaceship.StabilizationRotation;
            }
            
            Player.Spaceship.Move(_inputMove);
            Player.Spaceship.Rotate(_inputRotate);
            Player.Spaceship.Jet(_inputJet);
        }

        private void HandleCamera(float deltaTime)
        {
            if (_inputNextCamera)
            {
                VCam.NextCamera();
            }
            
            if (VCam.ActiveCamera)
            {
                _orthographicSize += -_inputZoom * _orthographicSize * zoomRate;

                _orthographicSize = Mathf.Clamp(_orthographicSize, 1f, 20f);
                
                VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize = _orthographicSize;
            }
        }

        private void Update()
        {
            HandleInput(Time.deltaTime);

            HandlePlayer();
            
            HandleCamera(Time.deltaTime);
        }
    }
}
