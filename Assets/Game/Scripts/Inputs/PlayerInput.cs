using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Inputs
{
    [CreateAssetMenu(menuName = "Game/Input/Create PlayerInput", fileName = "PlayerInput", order = 0)]
    public class PlayerInput : ScriptableObject, GameInput.ISpaceshipActions
    {
        public event Action<Vector2> MoveEvent; 
        public event Action<float> RotateEvent; 
        public event Action<float> JetEvent; 
        public event Action KeepRotationEvent; 
        public event Action KeepPositionEvent; 
        
        public event Action<bool> InteractEvent;
        
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

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
            }
            
            _gameInput.Spaceship.SetCallbacks(this);
            _gameInput.Spaceship.Enable();
        }

        private void OnDisable()
        {
            _gameInput.Spaceship.Disable();
            _gameInput.Spaceship.RemoveCallbacks(this);
        }
    }
}