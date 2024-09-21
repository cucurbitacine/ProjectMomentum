using System;
using Game.Scripts.Core;
using Game.Scripts.Levels;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Inputs
{
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInput : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Vector2 moveInput = Vector2.zero;
        [SerializeField] private float rotateInput = 0f;
        [SerializeField] private float jetInput = 0f;
        [SerializeField] private bool holdPosition = false;
        [SerializeField] private bool holdRotation = false;
        
        [Header("Player")]
        [SerializeField] private bool nextCamera = false;
        [SerializeField] private float zoomDelta = 0f;
        [SerializeField] private float zoomRate = 1f;
        
        private LazyComponent<PlayerController> _lazyPlayer;

        public PlayerController Player => (_lazyPlayer ??= new LazyComponent<PlayerController>(gameObject)).Value;

        [SerializeField] private float _orthographicSize = 6f;
        
        private void HandleInput()
        {
            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            
            rotateInput = 0f;
            rotateInput += Input.GetKey(KeyCode.E) ? -1f : 0f;
            rotateInput += Input.GetKey(KeyCode.Q) ? 1f : 0f;
            
            jetInput = Input.GetAxisRaw("Jump");

            holdPosition = Input.GetKeyDown(KeyCode.X);
            holdRotation = Input.GetKeyDown(KeyCode.R);
            
            nextCamera = Input.GetKeyDown(KeyCode.V);

            zoomDelta = Input.mouseScrollDelta.y;
        }
        
        private void HandlePlayer()
        {
            Player.Spaceship.Jet(jetInput);
            Player.Spaceship.Move(moveInput);
            Player.Spaceship.Rotate(rotateInput);

            if (holdPosition)
            {
                Player.Spaceship.HoldPosition = !Player.Spaceship.HoldPosition;
            }
            
            if (holdRotation)
            {
                Player.Spaceship.HoldRotation = !Player.Spaceship.HoldRotation;
            }
        }

        private void HandleCamera(float deltaTime)
        {
            if (nextCamera)
            {
                VCam.NextCamera();
            }
            
            if (VCam.ActiveCamera)
            {
                _orthographicSize += -zoomDelta * _orthographicSize * zoomRate;

                _orthographicSize = Mathf.Clamp(_orthographicSize, 1f, 20f);
                
                VCam.ActiveCamera.virtualCamera.m_Lens.OrthographicSize = _orthographicSize;
            }
        }

        private void Update()
        {
            HandleInput();

            HandlePlayer();
            
            HandleCamera(Time.deltaTime);
        }
    }
}
