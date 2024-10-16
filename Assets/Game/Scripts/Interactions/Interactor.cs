using System;
using System.Collections.Generic;
using CucuTools;
using UnityEngine;

namespace Game.Scripts.Interactions
{
    public class Interactor : MonoBehaviour, IPausable
    {
        [SerializeField] private bool interacting = false;

        public IInteractable Selected { get; private set; }
        public List<IInteractable> Interactions { get; } = new List<IInteractable>();

        public event Action<List<IInteractable>> ListUpdated;
        
        public void BeginInteract()
        {
            if (IsPaused) return;
            
            if (interacting) return;
            
            interacting = true;

            if (Interactions.Count == 0) return;
            
            Selected = Interactions[0];
            
            Selected.StartInteraction(gameObject);
        }
        
        public void EndInteract()
        {
            if (!interacting) return;
            
            interacting = false;
            
            Selected?.StopInteraction(gameObject);

            Selected = null;
        }
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            var root = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;
            
            if (root.TryGetComponent<IInteractable>(out var interaction))
            {
                if (!Interactions.Contains(interaction))
                {
                    Interactions.Add(interaction);
                    
                    ListUpdated?.Invoke(Interactions);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            var root = other.attachedRigidbody ? other.attachedRigidbody.transform : other.transform;
            
            if (root.TryGetComponent<IInteractable>(out var interaction))
            {
                if (Interactions.Remove(interaction))
                {
                    if (Selected == interaction)
                    {
                        Selected.StopInteraction(gameObject);
                    }
                    
                    ListUpdated?.Invoke(Interactions);
                }
            }
        }

        public bool IsPaused { get; private set; }
        
        public event Action<bool> Paused;

        public void Pause(bool value)
        {
            if (IsPaused == value) return;
            
            IsPaused = value;
            
            if (IsPaused)
            {
                EndInteract();
            }
            
            Paused?.Invoke(IsPaused);
        }
    }
}
