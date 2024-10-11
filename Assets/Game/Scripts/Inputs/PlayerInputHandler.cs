using Game.Scripts.Core;
using Game.Scripts.Levels;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Inputs
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float zoomRate = 1f;
        
        [Header("Input")]
        [SerializeField] private PlayerInput playerInput;

        [Header("References")]
        [SerializeField] private LevelController level;
        
        private float _zoom = 0f;
        private bool _isZooming = false;

        private LazyComponent<PlayerController> _lazyPlayer;

        private PlayerController player => (_lazyPlayer ??= new LazyComponent<PlayerController>(gameObject)).Value;

        private void HandleMove(Vector2 move)
        {
            player.Spaceship.Move(move);
        }

        private void HandleRotate(float rotate)
        {
            player.Spaceship.Rotate(rotate);
        }
        
        private void HandleJet(float jet)
        {
            player.Spaceship.Jet(jet);
        }

        private void HandleKeepRotation()
        {
            player.Spaceship.KeepRotation = !player.Spaceship.KeepRotation;
        }
        
        private void HandleKeepPosition()
        {
            player.Spaceship.KeepPosition = !player.Spaceship.KeepPosition;
        }

        private void HandleInteract(bool interact)
        {
            if (interact)
            {
                player.Interactor.BeginInteract();
            }
            else
            {
                player.Interactor.EndInteract();
            }
        }

        private void HandleZoom(float zoom)
        {
            _zoom = zoom;
            _isZooming = !Mathf.Approximately(zoom, 0f);
        }
        
        private void HandleChangeCamera()
        {
            VCam.NextCamera();
        }
        
        private void OnLevelPaused(bool paused)
        {
            player.Pause(paused);
        }
        
        private void OnEnable()
        {
            LevelPause.LevelPaused += OnLevelPaused;
            
            playerInput.MoveEvent += HandleMove;
            playerInput.RotateEvent += HandleRotate;
            playerInput.JetEvent += HandleJet;
            playerInput.KeepRotationEvent += HandleKeepRotation;
            playerInput.KeepPositionEvent += HandleKeepPosition;
            
            playerInput.InteractEvent += HandleInteract;

            playerInput.ZoomEvent += HandleZoom;
            playerInput.ChangeCameraEvent += HandleChangeCamera;
        }

        private void OnDisable()
        {
            LevelPause.LevelPaused -= OnLevelPaused;
            
            playerInput.MoveEvent -= HandleMove;
            playerInput.RotateEvent -= HandleRotate;
            playerInput.JetEvent -= HandleJet;
            playerInput.KeepRotationEvent -= HandleKeepRotation;
            playerInput.KeepPositionEvent -= HandleKeepPosition;
            
            playerInput.InteractEvent -= HandleInteract;
            
            playerInput.ZoomEvent -= HandleZoom;
            playerInput.ChangeCameraEvent -= HandleChangeCamera;
        }

        private void Update()
        {
            if (_isZooming)
            {
                if (VCam.ActiveCamera)
                {
                    var size = VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize;
                
                    size += -_zoom * zoomRate * size * Time.deltaTime;

                    size = Mathf.Clamp(size, 1f, 20f);
                
                    VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize = size;
                }
            }
        }
    }
}
