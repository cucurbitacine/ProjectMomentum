using CucuTools;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Inputs
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] private PlayerInput playerInput;
        
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
        
        private void OnEnable()
        {
            playerInput.MoveEvent += HandleMove;
            playerInput.RotateEvent += HandleRotate;
            playerInput.JetEvent += HandleJet;
            playerInput.KeepRotationEvent += HandleKeepRotation;
            playerInput.KeepPositionEvent += HandleKeepPosition;
            
            playerInput.InteractEvent += HandleInteract;
        }

        private void OnDisable()
        {
            playerInput.MoveEvent -= HandleMove;
            playerInput.RotateEvent -= HandleRotate;
            playerInput.JetEvent -= HandleJet;
            playerInput.KeepRotationEvent -= HandleKeepRotation;
            playerInput.KeepPositionEvent -= HandleKeepPosition;
            
            playerInput.InteractEvent -= HandleInteract;
        }
    }
}
