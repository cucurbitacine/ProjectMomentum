using System;
using System.Collections.Generic;
using Game.Scripts.Core;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.UI
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteButton : MonoBehaviour
    {
        [SerializeField] private bool interactable = true;
        [SerializeField] private Color baseColor = Color.white;
        
        [Space]
        [SerializeField] private Color normalColor = new Color(1.00f, 1.00f, 1.00f, 1.00f);
        [SerializeField] private Color highlightedColor = new Color(0.96f, 0.96f, 0.96f, 1.00f);
        [SerializeField] private Color pressedColor = new Color(0.78f, 0.78f, 0.78f, 1.00f);
        [SerializeField] private Color disabledColor = new Color(0.78f, 0.78f, 0.78f, 0.50f);
        
        [Space]
        [SerializeField] private UnityEvent onClick = new UnityEvent();
        [SerializeField] private UnityEvent onHover = new UnityEvent();

        [Space]
        [SerializeField] private List<AudioClip> clickSfx = new List<AudioClip>();
        [SerializeField] private List<AudioClip> hoverSfx = new List<AudioClip>();
        
        private LazyComponent<SpriteRenderer> _lazySprite;
        
        public bool Interactable
        {
            get => interactable;
            set
            {
                if (interactable == value) return;

                interactable = value;

                SetButtonState(interactable ? ButtonState.Normal : ButtonState.Disabled);
            }
        }

        public SpriteRenderer Sprite => (_lazySprite ??= new LazyComponent<SpriteRenderer>(gameObject)).Value;
        public ButtonState State { get; private set; } = ButtonState.Normal;
        
        public void Click()
        {
            onClick.Invoke();
        }
        
        private void UpdateButtonColor()
        {
            if (State == ButtonState.Normal)
            {
                Sprite.color = baseColor * normalColor;
            }
            
            if (State == ButtonState.Highlighted)
            {
                Sprite.color = baseColor * highlightedColor;
            }
            
            if (State == ButtonState.Pressed)
            {
                Sprite.color = baseColor * pressedColor;
            }
            
            if (State == ButtonState.Disabled)
            {
                Sprite.color = baseColor * disabledColor;
            }
        }

        private void SetButtonState(ButtonState nextState)
        {
            State = nextState;
            
            UpdateButtonColor();
        }

        [ContextMenu(nameof(EnableButton))]
        private void EnableButton()
        {
            Interactable = true;
        }
        
        [ContextMenu(nameof(DisableButton))]
        private void DisableButton()
        {
            Interactable = false;
        }
        
        private void OnMouseEnter()
        {
            if (State == ButtonState.Normal)
            {
                SetButtonState(ButtonState.Highlighted);
                
                gameObject.PlayOneShot(hoverSfx);
            }
        }

        private void OnMouseExit()
        {
            if (State == ButtonState.Highlighted)
            {
                SetButtonState(ButtonState.Normal);
            }
        }

        private void OnMouseDown()
        {
            if (State == ButtonState.Highlighted)
            {
                SetButtonState(ButtonState.Pressed);
                
                gameObject.PlayOneShot(clickSfx);
            }
        }

        private void OnMouseUpAsButton()
        {
            if (State == ButtonState.Pressed)
            {
                SetButtonState(ButtonState.Highlighted);
                
                Click();
            }
        }
        
        private void OnMouseUp()
        {
            if (State == ButtonState.Pressed)
            {
                SetButtonState(ButtonState.Normal);
            }
        }
        
        private void Awake()
        {
            baseColor = Sprite.color;
        }

        private void OnValidate()
        {
            if (Application.isEditor && !Application.isPlaying && TryGetComponent<SpriteRenderer>(out var sprite))
            {
                baseColor = sprite.color;
            }
        }

        public enum ButtonState
        {
            Normal,
            Highlighted,
            Pressed,
            Disabled,
        }
    }
}
