using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Inputs
{
    [CreateAssetMenu(menuName = "Game/Input/Create PlayerInput", fileName = "PlayerInput", order = 0)]
    public class PlayerInput : ScriptableObject, GameInput.ISpaceshipActions, GameInput.IInteractionActions, GameInput.ICameraActions
    {
        public event Action<Vector2> MoveEvent; 
        public event Action<float> RotateEvent; 
        public event Action<float> JetEvent; 
        public event Action KeepRotationEvent; 
        public event Action KeepPositionEvent; 
        
        public event Action<bool> InteractEvent;
        
        public event Action<float> ZoomEvent;
        public event Action ChangeCameraEvent;

        private GameInput _gameInput;
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveEvent?.Invoke(context.ReadValue<Vector2>());
        }
        
        public void OnRotate(InputAction.CallbackContext context)
        {
            RotateEvent?.Invoke(context.ReadValue<float>());
        }

        public void OnJet(InputAction.CallbackContext context)
        {
            JetEvent?.Invoke(context.ReadValue<float>());
        }
        
        public void OnKeepRotation(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                KeepRotationEvent?.Invoke();
            }
        }

        public void OnKeepPosition(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                KeepPositionEvent?.Invoke();
            }
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                InteractEvent?.Invoke(true);
            }
            else if (context.canceled)
            {
                InteractEvent?.Invoke(false);
            }
        }

        public void OnChangeCamera(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ChangeCameraEvent?.Invoke();
            }
        }

        public void OnZoom(InputAction.CallbackContext context)
        {
            ZoomEvent?.Invoke(context.ReadValue<float>());
        }
        
        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
            }
            
            _gameInput.Spaceship.SetCallbacks(this);
            _gameInput.Spaceship.Enable();
            
            _gameInput.Interaction.SetCallbacks(this);
            _gameInput.Interaction.Enable();
            
            _gameInput.Camera.SetCallbacks(this);
            _gameInput.Camera.Enable();
        }

        private void OnDisable()
        {
            _gameInput.Spaceship.Disable();
            _gameInput.Spaceship.RemoveCallbacks(this);
            
            _gameInput.Interaction.Disable();
            _gameInput.Interaction.RemoveCallbacks(this);
            
            _gameInput.Camera.Disable();
            _gameInput.Camera.RemoveCallbacks(this);
        }
    }
}