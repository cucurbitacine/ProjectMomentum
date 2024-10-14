using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Inputs
{
    [CreateAssetMenu(menuName = "Game/Input/Create CameraInput", fileName = "CameraInput", order = 0)]
    public class CameraInput : ScriptableObject, GameInput.ICameraActions
    {
        public event Action<float> ZoomEvent;
        public event Action ChangeCameraEvent;
        public event Action<float> AimEvent;
        
        private GameInput _gameInput;
        
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

        public void OnAim(InputAction.CallbackContext context)
        {
            AimEvent?.Invoke(context.ReadValue<float>());
        }
        
        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
            }
            
            _gameInput.Camera.SetCallbacks(this);
            _gameInput.Camera.Enable();
        }

        private void OnDisable()
        {
            _gameInput.Camera.Disable();
            _gameInput.Camera.RemoveCallbacks(this);
        }
    }
}