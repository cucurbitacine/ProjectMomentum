using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Inputs
{
    [CreateAssetMenu(menuName = "Game/Input/Create UIInput", fileName = "UIInput", order = 0)]
    public class UIInput : ScriptableObject, GameInput.IUIActions
    {
        public event Action MenuEvent;
        public event Action SubmitEvent;
        public event Action CancelEvent;
        public event Action<Vector2> NavigateEvent;

        private GameInput _gameInput;

        public void OnMenu(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                MenuEvent?.Invoke();
            }
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SubmitEvent?.Invoke();
            }
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                CancelEvent?.Invoke();
            }
        }

        public void OnNavigate(InputAction.CallbackContext context)
        {
            NavigateEvent?.Invoke(context.ReadValue<Vector2>());
        }

        private void OnEnable()
        {
            if (_gameInput == null)
            {
                _gameInput = new GameInput();
            }

            _gameInput.UI.SetCallbacks(this);
            _gameInput.UI.Enable();
        }

        private void OnDisable()
        {
            _gameInput.UI.Disable();
            _gameInput.UI.RemoveCallbacks(this);
        }
    }
}